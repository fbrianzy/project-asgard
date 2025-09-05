# Go CLI Calculator

A tiny yet practical command-line calculator written in Go (stdlib only).
Supports basic arithmetic and quick stats over a list of numbers — perfect for shell pipelines and scripting.

---

## ✨ Features

- Operations: add, sub, mul, div, avg, min, max
- Flexible input: angka dipisah spasi atau koma (campur juga bisa)
- Clear errors (mis. division by zero, input kosong, operasi tak dikenal)
- Clean layout: cmd/ untuk binary, internal/ untuk library + unit tests
- No external dependencies (pure flag, fmt, dll.)

---

## 📁 Project Layout
```bash
cli-calculator/
├── go.mod
├── cmd/
│   └── calculator/
│       └── main.go
└── internal/
    └── calc/
        ├── calc.go
        └── calc_test.go
```

---

## 🚀 Quick Start
```bash
cd go/cli-calculator
go mod tidy

# Run
go run ./cmd/calculator -op add -nums "1,2,3"

# Build binary
go build -o calculator ./cmd/calculator
./calculator -op avg -nums "2,4,6,8"
```

---

## 🧩 Usage
```bash
CLI Calculator (Go)

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

Flags:
  -op    operation: add|sub|mul|div|avg|min|max
  -nums  numbers, e.g. "1,2,3" atau "1 2 3"
```

---

## 🔢 Operations & Rules
| Operation | Keterangan                       | Minimal input |
| --------- | -------------------------------- | ------------- |
| `add`     | Penjumlahan                      | 1 angka       |
| `sub`     | Pengurangan **left-associative** | 2 angka       |
| `mul`     | Perkalian                        | 1 angka       |
| `div`     | Pembagian **left-associative**   | 2 angka       |
| `avg`     | Rata-rata                        | 1 angka       |
| `min`     | Nilai minimum                    | 1 angka       |
| `max`     | Nilai maksimum                   | 1 angka       |

```text
Contoh left-associative:
sub 10 2 3 → (10 - 2) - 3 = 5
div 20 2 2 → (20 / 2) / 2 = 5
```

---

## 🧪 Tests
```bash
# Jalankan unit test untuk paket kalkulasi
go test ./internal/calc -v
```

