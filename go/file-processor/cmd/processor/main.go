package main

import (
	"encoding/json"
	"flag"
	"fmt"
	"log"
	"os"
	"strings"

	"https://github.com/fbrianzy/project-asgard/tree/main/go/rest-api/internal/processor" // change the path
)

func main() {
	input := flag.String("in", "data/sample.csv", "input CSV file")
	cols := flag.String("cols", "Age,Score", "numeric columns to analyze (comma-separated)")
	outJSON := flag.String("out", "", "optional output JSON file")
	flag.Parse()

	rows, err := processor.ReadCSV(*input)
	if err != nil {
		log.Fatalf("error reading CSV: %v", err)
	}
	if len(rows) == 0 {
		log.Fatalf("no data in file")
	}

	var results []processor.Summary
	for _, col := range strings.Split(*cols, ",") {
		col = strings.TrimSpace(col)
		nums, err := processor.ExtractColumn(rows, col)
		if err != nil {
			log.Fatalf("extract %s: %v", col, err)
		}
		res := processor.ComputeStats(col, nums)
		results = append(results, res)
	}

	// Print to console
	for _, r := range results {
		fmt.Printf("Column: %s\n", r.Column)
		fmt.Printf("  Count: %d\n", r.Count)
		fmt.Printf("  Min  : %.2f\n", r.Min)
		fmt.Printf("  Max  : %.2f\n", r.Max)
		fmt.Printf("  Mean : %.2f\n", r.Mean)
		fmt.Println()
	}

	// Export JSON if requested
	if *outJSON != "" {
		if err := processor.ExportJSON(*outJSON, results); err != nil {
			log.Fatalf("export json: %v", err)
		}
		fmt.Println("Summary exported to", *outJSON)
	} else {
		// Pretty print JSON inline
		js, _ := json.MarshalIndent(results, "", "  ")
		fmt.Println(string(js))
	}
}
