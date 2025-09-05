# 🚀 Quick Start
```bash
cd go/file-processor
go mod tidy
```


## Run (use sample.csv)
```bash
go run ./cmd/processor -in data/sample.csv -cols "Age,Score"
```

## Export JSON
```bash
go run ./cmd/processor -in data/sample.csv -cols "Age,Score" -out result.json
```

# Example Output in Console
```bash
Column: Age
  Count: 5
  Min  : 22.00
  Max  : 30.00
  Mean : 25.60

Column: Score
  Count: 5
  Min  : 70.00
  Max  : 95.00
  Mean : 84.00
```