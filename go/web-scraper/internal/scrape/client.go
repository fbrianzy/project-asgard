package scrape

import (
	"context"
	"errors"
	"net"
	"net/http"
	"time"
)

type HTTPClient struct {
	c *http.Client
}

func NewHTTPClient(timeout time.Duration) *HTTPClient {
	tr := &http.Transport{
		Proxy:               http.ProxyFromEnvironment,
		DialContext:         (&net.Dialer{Timeout: 5 * time.Second}).DialContext,
		TLSHandshakeTimeout: 5 * time.Second,
		MaxIdleConns:        64,
		IdleConnTimeout:     30 * time.Second,
	}
	return &HTTPClient{
		c: &http.Client{
			Timeout:   timeout,
			Transport: tr,
		},
	}
}

func (hc *HTTPClient) Get(ctx context.Context, url string, ua string) (*http.Response, error) {
	req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if err != nil {
		return nil, err
	}
	if ua == "" {
		ua = "GoMiniScraper/1.0 (+github.com/yourname)"
	}
	req.Header.Set("User-Agent", ua)
	req.Header.Set("Accept", "text/html,application/xhtml+xml")
	res := struct {
		*http.Response
		err error
	}{}
	done := make(chan struct{})
	go func() {
		res.Response, res.err = hc.c.Do(req)
		close(done)
	}()
	select {
	case <-ctx.Done():
		return nil, errors.New("request canceled or timed out")
	case <-done:
		return res.Response, res.err
	}
}
