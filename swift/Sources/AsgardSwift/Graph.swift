import Foundation

// MARK: - Graph (Adjacency List)
final class Graph {
    struct Edge { let to: Int; let weight: Int }
    private(set) var adj: [[Edge]] = []

    init(_ n: Int) { adj = Array(repeating: [], count: n) }

    func addEdge(_ u: Int, _ v: Int, _ w: Int = 1, undirected: Bool = true) {
        guard u >= 0 && v >= 0 && u < adj.count && v < adj.count else { return }
        adj[u].append(Edge(to: v, weight: w))
        if undirected { adj[v].append(Edge(to: u, weight: w)) }
    }

    func bfs(from s: Int) -> [Int] {
        var dist = Array(repeating: -1, count: adj.count)
        var q: [Int] = [s]
        dist[s] = 0
        var head = 0
        while head < q.count {
            let u = q[head]; head += 1
            for e in adj[u] where dist[e.to] == -1 {
                dist[e.to] = dist[u] + 1
                q.append(e.to)
            }
        }
        return dist
    }

    func dfs(from s: Int) -> [Int] {
        var vis = Array(repeating: false, count: adj.count)
        var order: [Int] = []
        func go(_ u: Int) {
            vis[u] = true
            order.append(u)
            for e in adj[u] where !vis[e.to] { go(e.to) }
        }
        go(s)
        return order
    }

    func dijkstra(from s: Int) -> [Int] {
        let INF = Int.max / 4
        var dist = Array(repeating: INF, count: adj.count)
        dist[s] = 0
        var heap = BinaryHeap<(Int, Int)>(by: { $0.0 < $1.0 }) // (dist, node)
        heap.push((0, s))
        while let (d, u) = heap.pop() {
            if d != dist[u] { continue }
            for e in adj[u] {
                let nd = d + e.weight
                if nd < dist[e.to] {
                    dist[e.to] = nd
                    heap.push((nd, e.to))
                }
            }
        }
        return dist
    }
}

// Simple binary heap
struct BinaryHeap<T> {
    private var a: [T] = []
    private let cmp: (T, T) -> Bool
    init(by cmp: @escaping (T, T) -> Bool) { self.cmp = cmp }
    var isEmpty: Bool { a.isEmpty }
    mutating func push(_ x: T) { a.append(x); siftUp(a.count - 1) }
    mutating func pop() -> T? {
        guard !a.isEmpty else { return nil }
        a.swapAt(0, a.count - 1)
        let res = a.removeLast()
        if !a.isEmpty { siftDown(0) }
        return res
    }
    private mutating func siftUp(_ i0: Int) {
        var i = i0
        while i > 0 {
            let p = (i - 1) / 2
            if cmp(a[i], a[p]) { a.swapAt(i, p); i = p } else { break }
        }
    }
    private mutating func siftDown(_ i0: Int) {
        var i = i0
        while true {
            let l = 2 * i + 1, r = l + 1
            var best = i
            if l < a.count && cmp(a[l], a[best]) { best = l }
            if r < a.count && cmp(a[r], a[best]) { best = r }
            if best == i { break }
            a.swapAt(i, best); i = best
        }
    }
}

// MARK: - GraphCommand
struct GraphCommand: Runnable {
    static let command = "graph"
    static let description = "Graph algorithms demo. Options: --n <nodes> --m <edges> --weighted --from <s>"

    static func run(context: CommandContext) throws {
        let n = Int(context.flag("n", default: "8")!) ?? 8
        let m = Int(context.flag("m", default: String(max(7, n)))!) ?? max(7, n)
        let weighted = context.has("weighted")
        let s = Int(context.flag("from", default: "0")!) ?? 0
        var g = Graph(n)
        var rng = SystemRandomNumberGenerator()
        for _ in 0..<m {
            let u = Int.random(in: 0..<n, using: &rng)
            let v = Int.random(in: 0..<n, using: &rng)
            if u == v { continue }
            let w = weighted ? Int.random(in: 1...9, using: &rng) : 1
            g.addEdge(u, v, w, undirected: true)
        }
        let bfs = g.bfs(from: min(max(0, s), n-1))
        let dfs = g.dfs(from: min(max(0, s), n-1))
        let dij = g.dijkstra(from: min(max(0, s), n-1))
        print("BFS distances:", bfs.map { $0 == -1 ? -1 : $0 })
        print("DFS order:", dfs)
        if weighted { print("Dijkstra distances:", dij.map { $0 == Int.max/4 ? -1 : $0 }) }
        else { print("Dijkstra distances (unweighted behaves like BFS):", dij.map { $0 == Int.max/4 ? -1 : $0 }) }
    }
}