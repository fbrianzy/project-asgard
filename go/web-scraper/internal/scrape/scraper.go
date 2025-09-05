package scrape

import (
	"context"
	"fmt"
	"net/url"
	"strings"
	"time"
)

type Result struct {
	Page *Page  `json:"page"`
	Err  string `json:"error,omitempty"`
}

type Options struct {
	UserAgent string
	Delay     time.Duration // polite delay between requests
	Depth1    bool          // follow same-host links (depth 1)
}

func normalize(u string) (string, *url.URL, error) {
	parsed, err := url.Parse(u)
	if err != nil {
		return "", nil, err
	}
	if parsed.Scheme == "" {
		parsed.Scheme = "https"
	}
	return parsed.String(), parsed, nil
}

func sameHost(u1, u2 *url.URL) bool {
	return strings.EqualFold(u1.Hostname(), u2.Hostname())
}

func (o Options) delay(ctx context.Context) {
	if o.Delay <= 0 {
		return
	}
	t := time.NewTimer(o.Delay)
	select {
	case <-t.C:
	case <-ctx.Done():
	}
}

func Scrape(ctx context.Context, client *HTTPClient, rawURL string, opt Options) Result {
	uStr, _, err := normalize(rawURL)
	if err != nil {
		return Result{Err: fmt.Sprintf("invalid url: %v", err)}
	}
	opt.delay(ctx)

	res, err := client.Get(ctx, uStr, opt.UserAgent)
	if err != nil {
		return Result{Err: err.Error()}
	}
	defer res.Body.Close()
	if res.StatusCode < 200 || res.StatusCode >= 300 {
		return Result{Err: fmt.Sprintf("http status %d", res.StatusCode)}
	}
	page, err := ParseHTML(res.Body, uStr)
	if err != nil {
		return Result{Err: err.Error()}
	}
	return Result{Page: page}
}

func CrawlDepth1(ctx context.Context, client *HTTPClient, start string, opt Options, limit int) []Result {
	uStr, uParsed, err := normalize(start)
	if err != nil {
		return []Result{{Err: fmt.Sprintf("invalid url: %v", err)}}
	}
	first := Scrape(ctx, client, uStr, opt)
	results := []Result{first}
	if first.Page == nil || !opt.Depth1 {
		return results
	}
	count := 1
	for _, href := range first.Page.Links {
		if count >= limit {
			break
		}
		linkStr, linkURL, err := normalize(resolve(uStr, href))
		if err != nil {
			continue
		}
		if !sameHost(uParsed, linkURL) {
			continue
		}
		results = append(results, Scrape(ctx, client, linkStr, opt))
		count++
	}
	return results
}

// resolve href against base
func resolve(base, href string) string {
	bu, err := url.Parse(base)
	if err != nil {
		return href
	}
	hu, err := url.Parse(href)
	if err != nil {
		return href
	}
	return bu.ResolveReference(hu).String()
}
