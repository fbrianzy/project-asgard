package com.example.text;

import java.nio.file.*;
import java.util.*;
import java.util.stream.*;

public class Main {
    public static void main(String[] args) throws Exception {
        if(args.length==0){
            System.out.println("Usage: mvn -q exec:java -Dexec.args=\"path/to/file.txt 10\"");
            return;
        }
        Path p = Paths.get(args[0]);
        int top = args.length>1 ? Integer.parseInt(args[1]) : 10;

        String content = Files.readString(p).toLowerCase();
        String[] tokens = content.replaceAll("[^a-zA-Z0-9\\s]", " ").split("\\s+");

        Map<String, Long> freq = Arrays.stream(tokens)
                .filter(s -> !s.isBlank())
                .collect(Collectors.groupingBy(s->s, Collectors.counting()));

        System.out.println("Unique words: " + freq.size());
        freq.entrySet().stream()
                .sorted(Map.Entry.<String,Long>comparingByValue().reversed())
                .limit(top)
                .forEach(e -> System.out.printf("%-20s %d%n", e.getKey(), e.getValue()));
    }
}
