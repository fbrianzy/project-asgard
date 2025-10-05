import Foundation

// MARK: - Sorting Algorithms
enum Sorting {
    static func bubble<T: Comparable>(_ a: inout [T]) {
        guard a.count > 1 else { return }
        for i in 0..<(a.count - 1) {
            var swapped = false
            for j in 0..<(a.count - i - 1) {
                if a[j] > a[j+1] {
                    a.swapAt(j, j+1)
                    swapped = true
                }
            }
            if !swapped { break }
        }
    }

    static func insertion<T: Comparable>(_ a: inout [T]) {
        for i in 1..<a.count {
            let key = a[i]
            var j = i - 1
            while j >= 0 && a[j] > key {
                a[j + 1] = a[j]
                if j == 0 { j -= 1; break }
                j -= 1
            }
            if j >= 0 { a[j + 1] = key } else { a[0] = key }
        }
    }

    static func selection<T: Comparable>(_ a: inout [T]) {
        for i in 0..<(a.count - 1) {
            var minIdx = i
            for j in (i+1)..<a.count { if a[j] < a[minIdx] { minIdx = j } }
            if minIdx != i { a.swapAt(i, minIdx) }
        }
    }

    static func mergeSort<T: Comparable>(_ a: [T]) -> [T] {
        guard a.count > 1 else { return a }
        let mid = a.count / 2
        let left = mergeSort(Array(a[..<mid]))
        let right = mergeSort(Array(a[mid...]))
        return merge(left, right)
    }

    private static func merge<T: Comparable>(_ left: [T], _ right: [T]) -> [T] {
        var i = 0, j = 0, result: [T] = []
        result.reserveCapacity(left.count + right.count)
        while i < left.count && j < right.count {
            if left[i] <= right[j] { result.append(left[i]); i += 1 }
            else { result.append(right[j]); j += 1 }
        }
        if i < left.count { result.append(contentsOf: left[i...]) }
        if j < right.count { result.append(contentsOf: right[j...]) }
        return result
    }

    static func quickSort<T: Comparable>(_ a: inout [T]) {
        qsort(&a, 0, a.count - 1)
    }

    private static func qsort<T: Comparable>(_ a: inout [T], _ lo: Int, _ hi: Int) {
        if lo >= hi { return }
        let p = partition(&a, lo, hi)
        if p > 0 { qsort(&a, lo, p - 1) }
        qsort(&a, p + 1, hi)
    }

    private static func medianOfThree<T: Comparable>(_ a: inout [T], _ i: Int, _ j: Int, _ k: Int) -> Int {
        if a[i] < a[j] {
            if a[j] < a[k] { return j }
            return a[i] < a[k] ? k : i
        } else {
            if a[i] < a[k] { return i }
            return a[j] < a[k] ? k : j
        }
    }

    private static func partition<T: Comparable>(_ a: inout [T], _ lo: Int, _ hi: Int) -> Int {
        let m = (lo + hi) / 2
        let pivotIndex = medianOfThree(&a, lo, m, hi)
        a.swapAt(pivotIndex, hi)
        let pivot = a[hi]
        var i = lo
        for j in lo..<hi {
            if a[j] <= pivot { a.swapAt(i, j); i += 1 }
        }
        a.swapAt(i, hi)
        return i
    }
}

// MARK: - SortCommand
struct SortCommand: Runnable {
    static let command = "sort"
    static let description = "Run sorting algorithms. Options: --algo <bubble|insertion|selection|merge|quick> --size <N>"

    static func run(context: CommandContext) throws {
        let algo = context.flag("algo", default: "quick")!
        let size = Int(context.flag("size", default: "20")!) ?? 20
        var data = (0..<size).map { _ in Int.random(in: 0...9999) }
        let orig = data
        let t0 = Date()
        switch algo {
        case "bubble": Sorting.bubble(&data)
        case "insertion": Sorting.insertion(&data)
        case "selection": Sorting.selection(&data)
        case "merge": data = Sorting.mergeSort(data)
        case "quick": Sorting.quickSort(&data)
        default: throw CLIError.invalidArguments("Unknown algo \(algo)")
        }
        let dt = Date().timeIntervalSince(t0)
        guard data == orig.sorted() else { throw CLIError.runtime("Sorting failed integrity check") }
        print("Algorithm: \(algo) | Size: \(size) | Time: \(String(format: "%.4f", dt))s")
        if size <= 50 { print("Input: \(orig)\nSorted: \(data)") }
    }
}