package main

import (
	"context"
	"flag"
	"fmt"
	"sort"
	"time"

	"https://github.com/fbrianzy/project-asgard/tree/main/go/concurrency-demo/internal/pipeline" // change the path
)

func main() {
	total := flag.Int("n", 50, "how many integers to process (1..N)")
	workers := flag.Int("w", 4, "number of workers")
	delay := flag.Duration("throttle", 0, "optional throttle per item (e.g. 5ms, 100ms)")
	timeout := flag.Duration("timeout", 2*time.Second, "overall timeout (e.g. 3s, 1m)")
	flag.Parse()

	ctx, cancel := context.WithTimeout(context.Background(), *timeout)
	defer cancel()

	gen := pipeline.Generator(ctx, *total)
	out := pipeline.FanOut(ctx, gen, *workers)

	if *delay > 0 {
		out = pipeline.Throttle(ctx, out, *delay)
	}

	results := pipeline.Collect(ctx, out)

	// urutkan berdasarkan ID biar output rapi (worker pool menghasilkan out-of-order)
	sort.Slice(results, func(i, j int) bool { return results[i].ID < results[j].ID })

	ok, fail := 0, 0
	var totalSpent time.Duration
	for _, r := range results {
		if r.Err != nil {
			fail++
			fmt.Printf("ID=%02d  in=%d  ERROR=%v\n", r.ID, r.Input, r.Err)
			continue
		}
		ok++
		totalSpent += r.Spent
		fmt.Printf("ID=%02d  in=%d  out=%d  spent=%s\n", r.ID, r.Input, r.Output, r.Spent)
	}

	fmt.Println("\n--- SUMMARY ---")
	fmt.Printf("requested=%d  done=%d  errors=%d  workers=%d\n", *total, ok+fail, fail, *workers)
	if done := ok + fail; done > 0 {
		avg := totalSpent / time.Duration(ok)
		fmt.Printf("avg_worker_latency=%s\n", avg)
	} else {
		fmt.Println("no results (likely timed out)")
	}
}
