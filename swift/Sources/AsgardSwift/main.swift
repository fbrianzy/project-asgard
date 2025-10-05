import Foundation

// MARK: - ANSI Colors
enum Ansi {
    static let reset = "\u{001B}[0m"
    static let bold = "\u{001B}[1m"
    static let blue = "\u{001B}[34m"
    static let green = "\u{001B}[32m"
    static let yellow = "\u{001B}[33m"
    static let red = "\u{001B}[31m"
    static func wrap(_ s: String, _ color: String) -> String { color + s + reset }
}

// MARK: - Command Routing
struct CommandContext {
    let args: [String]
    func flag(_ name: String, default def: String? = nil) -> String? {
        if let i = args.firstIndex(of: "--" + name), i + 1 < args.count { return args[i+1] }
        return def
    }
    func has(_ name: String) -> Bool { args.contains("--" + name) }
}

protocol Runnable {
    static var command: String { get }
    static var description: String { get }
    static func run(context: CommandContext) throws
}

enum CLIError: Error, CustomStringConvertible {
    case invalidArguments(String)
    case runtime(String)
    var description: String {
        switch self {
        case .invalidArguments(let m): return "Invalid arguments: " + m
        case .runtime(let m): return "Runtime error: " + m
        }
    }
}

// Commands will be registered here
struct Router {
    private static var registry: [String: (CommandContext) throws -> Void] = [:]
    private static var help: [String: String] = [:]

    static func register<T: Runnable>(_ type: T.Type) {
        registry[T.command] = { try T.run(context: $0) }
        help[T.command] = T.description
    }

    static func run() {
        var arguments = CommandLine.arguments
        let program = (arguments.first ?? "asgard-swift")
        arguments.removeFirst()
        guard let name = arguments.first else {
            printSelfUsage(program: program)
            return
        }
        arguments.removeFirst()
        if name == "help" || name == "-h" || name == "--help" {
            printSelfUsage(program: program)
            return
        }
        guard let handler = registry[name] else {
            print(Ansi.wrap("Unknown command: \(name)\n", Ansi.red))
            printSelfUsage(program: program)
            return
        }
        let ctx = CommandContext(args: arguments)
        do {
            let t0 = Date()
            try handler(ctx)
            let dt = Date().timeIntervalSince(t0)
            fputs(Ansi.wrap(String(format: "\n✓ Completed in %.3fs\n", dt), Ansi.green), stderr)
        } catch let e as CLIError {
            fputs(Ansi.wrap("✗ \(e.description)\n", Ansi.red), stderr)
            exit(1)
        } catch {
            fputs(Ansi.wrap("✗ Unexpected: \(error)\n", Ansi.red), stderr)
            exit(2)
        }
    }

    private static func printSelfUsage(program: String) {
        print(Ansi.wrap("\nAsgardSwift CLI", Ansi.bold))
        print("Usage: \(program) <command> [options]\n")
        print("Commands:")
        for (k, v) in help.sorted(by: { $0.key < $1.key }) {
            let pad = String(repeating: " ", count: max(1, 14 - k.count))
            print("  \(Ansi.wrap(k, Ansi.blue))\(pad)\(v)")
        }
        print("\nRun '<command> --help' for details.")
    }
}

// MARK: - Entry
// Registration of commands happens via static initializers
@discardableResult
func registerAll() -> Bool {
    Router.register(SortCommand.self)
    Router.register(GraphCommand.self)
    Router.register(BSTCommand.self)
    Router.register(CSVCommand.self)
    Router.register(FileScanCommand.self)
    return true
}
let _ = registerAll()
Router.run()