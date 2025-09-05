package processor

import (
	"encoding/json"
	"math"
	"os"
)

type Summary struct {
	Column string  `json:"column"`
	Count  int     `json:"count"`
	Min    float64 `json:"min"`
	Max    float64 `json:"max"`
	Mean   float64 `json:"mean"`
}

func ComputeStats(col string, nums []float64) Summary {
	if len(nums) == 0 {
		return Summary{Column: col, Count: 0}
	}
	min := math.Inf(1)
	max := math.Inf(-1)
	sum := 0.0
	for _, n := range nums {
		if n < min {
			min = n
		}
		if n > max {
			max = n
		}
		sum += n
	}
	return Summary{
		Column: col,
		Count:  len(nums),
		Min:    min,
		Max:    max,
		Mean:   sum / float64(len(nums)),
	}
}

func ExportJSON(path string, data any) error {
	f, err := os.Create(path)
	if err != nil {
		return err
	}
	defer f.Close()
	enc := json.NewEncoder(f)
	enc.SetIndent("", "  ")
	return enc.Encode(data)
}
