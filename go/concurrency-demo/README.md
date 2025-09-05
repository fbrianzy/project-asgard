# Go Concurrency Demo

Showcase of core Go concurrency patterns using **only the standard library**:
- Goroutines & Channels
- **Fan-out / Fan-in** (worker pool)
- **Throttling** with `time.Ticker`
- **Cancellation & Timeouts** with `context`

> CPU-bound demo: compute squares of integers in parallel with simulated jitter.

---

## 📁 Layout
```bash
concurrency-demo/
├── cmd/
│ └── demo/
│ └── main.go
└── internal/
└── pipeline/
├── pipeline.go
└── pipeline_test.go
```

---


---

## 🚀 Run

```bash
cd go/concurrency-demo
go mod tidy

# Default: N=50, workers=4, timeout=2s
go run ./cmd/demo

# Custom: 200 tasks, 8 workers, throttle 2ms, timeout 5s
go run ./cmd/demo -n 200 -w 8 -throttle 2ms -timeout 5s
```

---

## Sample output (truncated):
```bash
ID=01  in=1  out=1   spent=2.245ms
ID=02  in=2  out=4   spent=1.337ms
...
--- SUMMARY ---
requested=50  done=50  errors=0  workers=4
avg_worker_latency=2.011ms
```

---

## Test
```bash
go test ./internal/pipeline -v
```

Tests cover:
- End-to-end pipeline correctness.
- Early cancellation (timeout) behavior.

---

## 🧭 Notes

- Results are produced out-of-order by design (parallel workers). We sort in main.go only for pretty printing.
- Switch to an I/O-bound job easily (e.g., file read or HTTP) — the patterns remain the same.
- This demo is self-contained; no external dependencies or network calls.
