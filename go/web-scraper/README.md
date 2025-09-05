# Go Web Scraper

Minimalistic web scraper using Go with:
- Custom **User-Agent**, **timeouts**, and **polite delay**
- Parse `<title>`, `<meta name="description">`, headings **H1–H3**, and `<a>` links
- Optional **same-host crawl** (depth = 1, limited by `-limit`)
- Export **JSON** and **CSV** summaries

> HTML parsing via `golang.org/x/net/html`. No heavy frameworks.

---

## 📁 Layout
```bash
web-scraper/
├── cmd/scraper/main.go
└── internal/scrape/
├── client.go
├── parser.go
├── scraper.go
└── export.go
```

---

## 🚀 Quick Start
```bash
cd go-projects/05-web-scraper
go mod tidy

# Single page
go run ./cmd/scraper -url https://example.com -out-json pages.json -out-csv summary.csv

# Depth-1 same-host crawl (max 8 pages), gentle delay
go run ./cmd/scraper -url https://example.com -depth1 -limit 8 -delay 300ms -out-json pages.json
```

## Sample console output:
```bash
URL: https://example.com
Title: Example Domain
Desc: ...
H1: [Example Domain]
Links: 1

JSON saved to pages.json
CSV saved to summary.csv
```

## ⚙️ Flags
| Flag        | Default             | Description                      |
| ----------- | ------------------- | -------------------------------- |
| `-url`      | (req)               | Start URL                        |
| `-depth1`   | `false`             | Follow same-host links (depth=1) |
| `-limit`    | `8`                 | Max pages when `-depth1` enabled |
| `-delay`    | `500ms`             | Polite delay between requests    |
| `-timeout`  | `10s`               | Global timeout for entire run    |
| `-ua`       | `GoMiniScraper/1.0` | Custom User-Agent                |
| `-out-json` | `""`                | Write raw results array to JSON  |
| `-out-csv`  | `""`                | Write compact summary CSV        |