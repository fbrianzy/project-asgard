package main

import (
	"context"
	"flag"
	"fmt"
	"log"
	"os"
	"time"

	"https://github.com/fbrianzy/project-asgard/tree/main/go/web-scraper/internal/scrape" // change the path
)

func main() {
	startURL := flag.String("url", "", "start URL (e.g. https://example.com)")
	depth1 := flag.Bool("depth1", false, "follow same-host links (depth=1)")
	limit := flag.Int("limit", 8, "max pages when depth1 enabled")
	delay := flag.Duration("delay", 500*time.Millisecond, "polite delay between requests (e.g. 200ms, 1s)")
	timeout := flag.Duration("timeout", 10*time.Second, "global timeout")
	ua := flag.String("ua", "GoMiniScraper/1.0 (+github.com/yourname)", "User-Agent")
	outJSON := flag.String("out-json", "", "write results to JSON file")
	outCSV := flag.String("out-csv", "", "write summary to CSV file")
	flag.Parse()

	if *startURL == "" {
		fmt.Println("Usage: scraper -url https://example.com [-depth1] [-limit 8] [-delay 300ms] [-out-json pages.json] [-out-csv summary.csv]")
		os.Exit(2)
	}

	ctx, cancel := context.WithTimeout(context.Background(), *timeout)
	defer cancel()

	client := scrape.NewHTTPClient(8 * time.Second)
	opt := scrape.Options{
		UserAgent: *ua,
		Delay:     *delay,
		Depth1:    *depth1,
	}

	var results []scrape.Result
	if *depth1 {
		results = scrape.CrawlDepth1(ctx, client, *startURL, opt, *limit)
	} else {
		results = []scrape.Result{scrape.Scrape(ctx, client, *startURL, opt)}
	}

	// print quick summary
	okPages := []*scrape.Page{}
	for _, r := range results {
		if r.Page != nil {
			okPages = append(okPages, r.Page)
			fmt.Printf("\nURL: %s\nTitle: %s\nDesc: %s\nH1: %v\nLinks: %d\n",
				r.Page.URL, r.Page.Title, r.Page.Description, r.Page.H1, len(r.Page.Links))
		} else {
			log.Printf("ERR: %s", r.Err)
		}
	}

	// exports
	if *outJSON != "" {
		if err := scrape.SaveJSON(*outJSON, results); err != nil {
			log.Fatalf("save json: %v", err)
		}
		fmt.Println("JSON saved to", *outJSON)
	}
	if *outCSV != "" {
		if err := scrape.SaveCSV(*outCSV, okPages); err != nil {
			log.Fatalf("save csv: %v", err)
		}
		fmt.Println("CSV saved to", *outCSV)
	}
}
