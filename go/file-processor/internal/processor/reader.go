package processor

import (
	"encoding/csv"
	"fmt"
	"io"
	"os"
	"strconv"
)

func ReadCSV(path string) ([]map[string]string, error) {
	f, err := os.Open(path)
	if err != nil {
		return nil, fmt.Errorf("open file: %w", err)
	}
	defer f.Close()

	r := csv.NewReader(f)
	headers, err := r.Read()
	if err != nil {
		return nil, fmt.Errorf("read headers: %w", err)
	}

	var records []map[string]string
	for {
		row, err := r.Read()
		if err == io.EOF {
			break
		}
		if err != nil {
			return nil, fmt.Errorf("read row: %w", err)
		}

		rec := make(map[string]string)
		for i, h := range headers {
			rec[h] = row[i]
		}
		records = append(records, rec)
	}
	return records, nil
}

// ExtractColumn mengubah kolom string → slice float64
func ExtractColumn(rows []map[string]string, col string) ([]float64, error) {
	var nums []float64
	for _, r := range rows {
		val := r[col]
		f, err := strconv.ParseFloat(val, 64)
		if err != nil {
			return nil, fmt.Errorf("invalid number %q in col %s: %w", val, col, err)
		}
		nums = append(nums, f)
	}
	return nums, nil
}
