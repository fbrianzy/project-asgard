package com.example.urlchecker;

import java.net.URI;
import java.net.http.*;
import java.time.Duration;
import java.util.*;
import java.util.concurrent.CompletableFuture;

public class Main {
    public static void main(String[] args) {
        if(args.length==0){
            System.out.println("Contoh: mvn -q exec:java -Dexec.args=\"https://example.com https://google.com\"");
            return;
        }
        HttpClient client = HttpClient.newBuilder().connectTimeout(Duration.ofSeconds(5)).build();
        List<String> urls = Arrays.asList(args);
        List<CompletableFuture<Void>> futures = new ArrayList<>();

        long start = System.currentTimeMillis();
        for(String url: urls){
            HttpRequest req = HttpRequest.newBuilder(URI.create(url)).timeout(Duration.ofSeconds(10)).GET().build();
            var fut = client.sendAsync(req, HttpResponse.BodyHandlers.discarding())
                    .thenAccept(res -> System.out.printf("%-40s -> %d%n", url, res.statusCode()))
                    .exceptionally(ex -> { System.out.printf("%-40s -> ERROR: %s%n", url, ex.getMessage()); return null; });
            futures.add(fut);
        }
        CompletableFuture.allOf(futures.toArray(new CompletableFuture[0])).join();
        System.out.println("Selesai dalam " + (System.currentTimeMillis()-start) + " ms");
    }
}
