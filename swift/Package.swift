// swift-tools-version:5.9
import PackageDescription

let package = Package(
    name: "AsgardSwift",
    platforms: [
        .macOS(.v12)
    ],
    products: [
        .executable(name: "asgard-swift", targets: ["AsgardSwift"]),
    ],
    targets: [
        .executableTarget(
            name: "AsgardSwift",
            path: "Sources/AsgardSwift"
        )
    ]
)