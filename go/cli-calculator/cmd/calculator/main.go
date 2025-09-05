package main

import (
	"errors"
	"flag"
	"fmt"
	"os"
	"strconv"
	"strings"

	"https://github.com/fbrianzy/project-asgard/tree/main/go/cli-calculator/internal/calc" // change the path
)

func parseNums(arg string) ([]float64, error) {
	arg = strings.TrimSpace(arg)
	if arg == "" {
		return nil, calc.ErrEmptyInput
	}

	// Support:
	// - comma-separated: "1,2,3"
	// - space-separated: "1 2 3"
	// - mixed commas & spaces: "1, 2  ,3"
	parts := strings.Fields(strings.ReplaceAll(arg, ",", " "))
	nums := make([]float64, 0, len(parts))
	for _, p := range parts {
		f, err := strconv.ParseFloat(p, 64)
		if err != nil {
			return nil, fmt.Errorf("invalid number %q: %w", p, err)
		}
		nums = append(nums, f)
	}
	return nums, nil
}

func usage() {
	fmt.Fprintf(os.Stderr, `CLI Calculator (Go)

Usage:
  calculator -op OPERATION -nums "NUMBERS"

Examples:
  calculator -op add -nums "1,2,3"        # 6
  calculator -op sub -nums "10 2 3"       # 5
  calculator -op mul -nums "2, 3, 4"      # 24
  calculator -op div -nums "20 2 2"       # 5
  calculator -op avg -nums "2,4,6,8"      # 5
  calculator -op min -nums "5 -1 3"       # -1
  calculator -op max -nums "5 -1 3"       # 5

Operations:
  add | sub | mul | div | avg | min | max

Flags:
`)
	flag.PrintDefaults()
}

func main() {
	opStr := flag.String("op", "", "operation: add|sub|mul|div|avg|min|max")
	numStr := flag.String("nums", "", `numbers, e.g. "1,2,3" or "1 2 3"`)
	flag.Usage = usage
	flag.Parse()

	if *opStr == "" {
		usage()
		os.Exit(2)
	}

	nums, err := parseNums(*numStr)
	if err != nil {
		fmt.Fprintln(os.Stderr, "error:", err)
		os.Exit(2)
	}

	res, err := calc.Compute(calc.Op(*opStr), nums)
	if err != nil {
		// berikan pesan yang ramah
		var unknownOp bool = errors.Is(err, calc.ErrUnknownOp)
		if unknownOp {
			fmt.Fprintln(os.Stderr, "error: unknown operation; valid: add|sub|mul|div|avg|min|max")
		} else {
			fmt.Fprintln(os.Stderr, "error:", err)
		}
		os.Exit(2)
	}

	// Output numerik polos biar gampang dipipe ke tools lain
	fmt.Println(res)
}
