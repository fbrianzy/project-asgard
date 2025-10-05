import Foundation

struct CSV {
    static func parse(_ text: String, delimiter: Character = ",") -> [[String]] {
        var rows: [[String]] = []
        var row: [String] = []
        var field = ""
        var insideQuotes = false
        for ch in text {
            if ch == "\""{ insideQuotes.toggle(); continue }
            if ch == delimiter && !insideQuotes {
                row.append(field); field.removeAll()
            } else if ch == "\n" && !insideQuotes {
                row.append(field); rows.append(row); row.removeAll(); field.removeAll()
            } else {
                field.append(ch)
            }
        }
        if !field.isEmpty || !row.isEmpty { row.append(field); rows.append(row) }
        return rows
    }

    static func toNumbers(_ rows: [[String]]) -> ([[Double?]], [Bool]) {
        var numericMask: [Bool] = []
        if let first = rows.first {
            for j in 0..<first.count {
                var allNumeric = true
                for i in 1..<rows.count {
                    if Double(rows[i][j]) == nil { allNumeric = false; break }
                }
                numericMask.append(allNumeric)
            }
        }
        let nums: [[Double?]] = rows.map { row in
            row.enumerated().map { (j, s) in numericMask.indices.contains(j) && numericMask[j] ? Double(s) : nil }
        }
        return (nums, numericMask)
    }

    static func describe(_ rows: [[String]]) -> String {
        guard !rows.isEmpty else { return "Empty CSV" }
        let headers = rows[0]
        let (nums, mask) = toNumbers(rows)
        var lines: [String] = []
        lines.append("Rows: \(rows.count - 1); Columns: \(headers.count)")
        for (j, h) in headers.enumerated() {
            if mask[j] {
                var vals: [Double] = []
                for i in 1..<rows.count { if let v = nums[i][j] { vals.append(v) } }
                if vals.isEmpty { lines.append("\(h): numeric but empty"); continue }
                let n = Double(vals.count)
                let mean = vals.reduce(0,+) / n
                let sorted = vals.sorted()
                let median = sorted.count % 2 == 1 ? sorted[sorted.count/2] : (sorted[sorted.count/2-1] + sorted[sorted.count/2]) / 2
                let minv = sorted.first ?? .nan
                let maxv = sorted.last ?? .nan
                let variance = vals.map { ($0 - mean) * ($0 - mean) }.reduce(0,+) / max(1, n - 1)
                let std = sqrt(variance)
                lines.append("\(h): min=\(minv), max=\(maxv), mean=\(String(format: "%.3f", mean)), median=\(String(format: "%.3f", median)), std=\(String(format: "%.3f", std))")
            } else {
                var unique: Set<String> = []
                for i in 1..<rows.count { unique.insert(rows[i][j]) }
                lines.append("\(h): categorical, unique=\(unique.count)")
            }
        }
        return lines.joined(separator: "\n")
    }

    static func readFile(at path: String) throws -> String {
        let url = URL(fileURLWithPath: path)
        return try String(contentsOf: url, encoding: .utf8)
    }
}

// MARK: - CSVCommand
struct CSVCommand: Runnable {
    static let command = "csv"
    static let description = "CSV quick summary. Options: --path <file.csv> --delimiter <,|;|\t>"

    static func run(context: CommandContext) throws {
        guard let path = context.flag("path") else {
            throw CLIError.invalidArguments("Missing --path <file.csv>")
        }
        let delimStr = context.flag("delimiter", default: ",")!
        let delimiter: Character = delimStr == "\t" ? "\t" : (delimStr.first ?? ",")
        let text = try CSV.readFile(at: path)
        let rows = CSV.parse(text, delimiter: delimiter)
        print(CSV.describe(rows))
        if context.has("head") {
            let k = min(rows.count, 5)
            print("\nHead:")
            for i in 0..<k { print(rows[i]) }
        }
    }
}