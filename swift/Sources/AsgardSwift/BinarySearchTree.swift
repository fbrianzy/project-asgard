import Foundation

final class BinarySearchTree<T: Comparable>: CustomStringConvertible {
    final class Node {
        var key: T
        var left: Node?
        var right: Node?
        init(_ key: T) { self.key = key }
    }
    private(set) var root: Node?

    func insert(_ key: T) {
        root = insert(root, key)
    }

    private func insert(_ node: Node?, _ key: T) -> Node {
        guard let node = node else { return Node(key) }
        if key < node.key {
            node.left = insert(node.left, key)
        } else if key > node.key {
            node.right = insert(node.right, key)
        }
        return node
    }

    func contains(_ key: T) -> Bool {
        var cur = root
        while let n = cur {
            if key == n.key { return true }
            cur = key < n.key ? n.left : n.right
        }
        return false
    }

    func min() -> T? {
        var cur = root
        var last: Node?
        while let n = cur { last = n; cur = n.left }
        return last?.key
    }

    func max() -> T? {
        var cur = root
        var last: Node?
        while let n = cur { last = n; cur = n.right }
        return last?.key
    }

    func remove(_ key: T) {
        root = remove(root, key)
    }

    private func remove(_ node: Node?, _ key: T) -> Node? {
        guard let node = node else { return nil }
        if key < node.key {
            node.left = remove(node.left, key)
        } else if key > node.key {
            node.right = remove(node.right, key)
        } else {
            // found
            if node.left == nil { return node.right }
            if node.right == nil { return node.left }
            // replace with successor
            if let s = minNode(node.right) {
                node.key = s.key
                node.right = remove(node.right, s.key)
            }
        }
        return node
    }

    private func minNode(_ node: Node?) -> Node? {
        var cur = node
        var last: Node?
        while let n = cur { last = n; cur = n.left }
        return last
    }

    func inorder() -> [T] { var r: [T] = []; inorder(root, &r); return r }
    private func inorder(_ node: Node?, _ out: inout [T]) {
        guard let n = node else { return }
        inorder(n.left, &out); out.append(n.key); inorder(n.right, &out)
    }

    func pretty() -> String {
        var lines: [String] = []
        func go(_ node: Node?, _ prefix: String, _ isLeft: Bool) {
            guard let n = node else { return }
            lines.append(prefix + (isLeft ? "├── " : "└── ") + "\(n.key)")
            go(n.left, prefix + (isLeft ? "│   " : "    "), true)
            go(n.right, prefix + (isLeft ? "│   " : "    "), false)
        }
        go(root, "", false)
        return lines.joined(separator: "\n")
    }

    var description: String { pretty() }
}

// MARK: - BSTCommand
struct BSTCommand: Runnable {
    static let command = "bst"
    static let description = "Binary Search Tree demo. Options: --values "1,5,2,9" --contains <x> --remove <x>"

    static func run(context: CommandContext) throws {
        let str = context.flag("values", default: "7,3,9,1,5,8,10,4,6")!
        let values = str.split(separator: ",").compactMap { Int($0.trimmingCharacters(in: .whitespaces)) }
        var bst = BinarySearchTree<Int>()
        for v in values { bst.insert(v) }
        if let q = context.flag("contains") { print("contains(\(q)) ->", bst.contains(Int(q) ?? Int.min)) }
        if let r = context.flag("remove"), let rv = Int(r) { bst.remove(rv) }
        print("inorder:", bst.inorder())
        print("min:", bst.min() ?? -1, "max:", bst.max() ?? -1)
        print("tree:\n\(bst.pretty())")
    }
}