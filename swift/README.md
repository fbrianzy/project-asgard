# AsgardSwift (Project Asgard · Swift Edition)

> A code-heavy Swift chapter for **Project Asgard**. Multi-file CLI with classic data structures, algorithms, CSV utilities, and file scanning — each file 100+ LOC to match your “gemuk kode” style.

## Highlights
- Pure Swift (no external dependencies), SwiftPM package.
- Commands:
  - `sort` – Bubble/Insertion/Selection/Merge/Quick with integrity check + timing.
  - `graph` – BFS/DFS/Dijkstra over random graph (binary heap included).
  - `bst` – Binary Search Tree insert/search/remove + pretty print.
  - `csv` – Tiny CSV parser + numeric stats (min/max/mean/median/std).
  - `fs` – Recursive directory listing / grep with optional JSON output.

## Structure
```text
AsgardSwift/
├── Package.swift
└── Sources/
    └── AsgardSwift/
        ├── main.swift              # CLI router (help, timing, registration)
        ├── Sorting.swift           # 150+ LOC: 5 algos + benchmark
        ├── Graph.swift             # 150+ LOC: Graph + BFS/DFS/Dijkstra + BinaryHeap
        ├── BinarySearchTree.swift  # 140+ LOC: BST insert/find/remove/pretty
        ├── CSV.swift               # 130+ LOC: CSV parser + describe
        └── FileScan.swift          # 120+ LOC: list/grep/json
```

## Build & Run
```powershell
swift build
swift run asgard-swift help

swift run asgard-swift sort --algo quick --size 30
swift run asgard-swift graph --n 10 --m 14 --weighted --from 0
swift run asgard-swift bst --values "7,3,9,1,5,8,10,4,6" --contains 5 --remove 3
swift run asgard-swift csv --path ./sample.csv --delimiter ,
swift run asgard-swift fs --path . --grep "swift" --json
```

> Tested locally on Swift 5.9+ (macOS). Should also work on Linux with Swift toolchain installed.
