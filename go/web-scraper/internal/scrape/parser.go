package scrape

import (
	"bytes"
	"io"
	"strings"

	"golang.org/x/net/html"
)

type Page struct {
	URL         string   `json:"url"`
	Title       string   `json:"title"`
	Description string   `json:"description"`
	H1          []string `json:"h1"`
	H2          []string `json:"h2"`
	H3          []string `json:"h3"`
	Links       []string `json:"links"` // raw href values
}

func text(n *html.Node) string {
	var buf bytes.Buffer
	var f func(*html.Node)
	f = func(nd *html.Node) {
		if nd.Type == html.TextNode {
			buf.WriteString(nd.Data)
		}
		for c := nd.FirstChild; c != nil; c = c.NextSibling {
			f(c)
		}
	}
	f(n)
	return strings.TrimSpace(strings.Join(strings.Fields(buf.String()), " "))
}

func getAttr(n *html.Node, key string) string {
	for _, a := range n.Attr {
		if strings.EqualFold(a.Key, key) {
			return a.Val
		}
	}
	return ""
}

func ParseHTML(body io.Reader, baseURL string) (*Page, error) {
	root, err := html.Parse(body)
	if err != nil {
		return nil, err
	}
	p := &Page{URL: baseURL}

	var f func(*html.Node)
	f = func(n *html.Node) {
		if n.Type == html.ElementNode {
			switch strings.ToLower(n.Data) {
			case "title":
				if p.Title == "" {
					p.Title = text(n)
				}
			case "meta":
				name := strings.ToLower(getAttr(n, "name"))
				prop := strings.ToLower(getAttr(n, "property"))
				if name == "description" || prop == "og:description" {
					if p.Description == "" {
						p.Description = getAttr(n, "content")
					}
				}
			case "h1":
				p.H1 = append(p.H1, text(n))
			case "h2":
				p.H2 = append(p.H2, text(n))
			case "h3":
				p.H3 = append(p.H3, text(n))
			case "a":
				href := getAttr(n, "href")
				if href != "" {
					p.Links = append(p.Links, href)
				}
			}
		}
		for c := n.FirstChild; c != nil; c = c.NextSibling {
			f(c)
		}
	}
	f(root)
	return p, nil
}
