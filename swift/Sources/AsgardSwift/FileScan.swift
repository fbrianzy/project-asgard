import Foundation

struct FileInfo: Codable {
    let path: String
    let size: UInt64
    let modified: Date
    let isDirectory: Bool
}

struct FileScanner {
    static func list(at path: String, recursive: Bool = true) -> [FileInfo] {
        var out: [FileInfo] = []
        let fm = FileManager.default
        func visit(_ p: String) {
            var isDir: ObjCBool = false
            if fm.fileExists(atPath: p, isDirectory: &isDir) {
                if isDir.boolValue {
                    if recursive, let children = try? fm.contentsOfDirectory(atPath: p) {
                        for c in children { visit((p as NSString).appendingPathComponent(c)) }
                    }
                    let attrs = try? fm.attributesOfItem(atPath: p)
                    let size = (attrs?[.size] as? UInt64) ?? 0
                    let m = (attrs?[.modificationDate] as? Date) ?? Date(timeIntervalSince1970: 0)
                    out.append(FileInfo(path: p, size: size, modified: m, isDirectory: true))
                } else {
                    let attrs = try? fm.attributesOfItem(atPath: p)
                    let size = (attrs?[.size] as? UInt64) ?? 0
                    let m = (attrs?[.modificationDate] as? Date) ?? Date(timeIntervalSince1970: 0)
                    out.append(FileInfo(path: p, size: size, modified: m, isDirectory: false))
                }
            }
        }
        visit(path)
        return out
    }

    static func grep(at path: String, pattern: String) -> [(String, Int, String)] {
        var results: [(String, Int, String)] = []
        let fm = FileManager.default
        let enumerator = fm.enumerator(atPath: path)
        while let item = enumerator?.nextObject() as? String {
            if item.hasSuffix(".swift") || item.hasSuffix(".txt") || item.hasSuffix(".md") {
                let full = (path as NSString).appendingPathComponent(item)
                if let data = try? String(contentsOfFile: full, encoding: .utf8) {
                    let lines = data.split(separator: "\n", omittingEmptySubsequences: false)
                    for (i, line) in lines.enumerated() {
                        if line.localizedCaseInsensitiveContains(pattern) {
                            results.append((full, i + 1, String(line)))
                        }
                    }
                }
            }
        }
        return results
    }
}

struct FileScanCommand: Runnable {
    static let command = "fs"
    static let description = "Scan files. Options: --path <dir> --grep <pattern> --json"

    static func run(context: CommandContext) throws {
        let path = context.flag("path", default: ".")!
        if let p = context.flag("grep") {
            let matches = FileScanner.grep(at: path, pattern: p)
            if matches.isEmpty { print("No matches for '\(p)' in \(path)") }
            else {
                for (file, line, text) in matches {
                    print("\(file):\(line): \(text)")
                }
            }
        } else {
            let items = FileScanner.list(at: path, recursive: true)
            if context.has("json") {
                let enc = JSONEncoder()
                enc.outputFormatting = [.prettyPrinted, .sortedKeys]
                enc.dateEncodingStrategy = .iso8601
                let data = try enc.encode(items)
                if let s = String(data: data, encoding: .utf8) { print(s) }
            } else {
                for i in items {
                    let type = i.isDirectory ? "[D]" : "[F]"
                    print("\(type) \(i.path) \t \(i.size) bytes \t \(i.modified)")
                }
            }
        }
    }
}