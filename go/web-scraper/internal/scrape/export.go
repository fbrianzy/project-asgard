package scrape

import (
	"encoding/csv"
	"encoding/json"
	"os"
)

func SaveJSON(path string, v any) error {
	f, err := os.Create(path)
	if err != nil {
		return err
	}
	defer f.Close()
	enc := json.NewEncoder(f)
	enc.SetIndent("", "  ")
	return enc.Encode(v)
}

func SaveCSV(path string, pages []*Page) error {
	f, err := os.Create(path)
	if err != nil {
		return err
	}
	defer f.Close()
	w := csv.NewWriter(f)
	defer w.Flush()

	_ = w.Write([]string{
		"url", "title", "description", "h1_joined", "h2_joined", "h3_joined", "links_count",
	})
	for _, p := range pages {
		_ = w.Write([]string{
			p.URL,
			p.Title,
			p.Description,
			join(p.H1),
			join(p.H2),
			join(p.H3),
			itostr(len(p.Links)),
		})
	}
	return w.Error()
}

func join(ss []string) string {
	out := ""
	for i, s := range ss {
		if i > 0 {
			out += " | "
		}
		out += s
	}
	return out
}

func itostr(n int) string {
	return json.Number(json.MarshalNumber(n)).String()
}
