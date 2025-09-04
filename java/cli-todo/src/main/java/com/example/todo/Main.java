package com.example.todo;

import java.io.IOException;
import java.nio.file.*;
import java.time.LocalDateTime;
import java.util.*;
import java.util.stream.Collectors;

public class Main {
    static final Path DB = Paths.get("tasks.csv");

    record Task(int id, String text, boolean done, LocalDateTime createdAt) {
        String toCsv() { return id + "," + escape(text) + "," + done + "," + createdAt; }
        static Task fromCsv(String line) {
            String[] p = splitCsv(line);
            return new Task(Integer.parseInt(p[0]), unescape(p[1]), Boolean.parseBoolean(p[2]), LocalDateTime.parse(p[3]));
        }
        static String escape(String s){ return s.replace("\\","\\\\").replace(",","\\,"); }
        static String unescape(String s){
            StringBuilder out = new StringBuilder(); boolean esc = false;
            for(char c: s.toCharArray()){
                if(esc){ out.append(c); esc=false; }
                else if(c=='\\'){ esc=true; }
                else out.append(c);
            } return out.toString();
        }
        static String[] splitCsv(String line){
            List<String> parts=new ArrayList<>(); StringBuilder cur=new StringBuilder(); boolean esc=false;
            for(char c: line.toCharArray()){
                if(esc){ cur.append(c); esc=false; }
                else if(c=='\\'){ esc=true; }
                else if(c==','){ parts.add(cur.toString()); cur.setLength(0); }
                else cur.append(c);
            } parts.add(cur.toString());
            return parts.toArray(new String[0]);
        }
    }

    public static void main(String[] args) throws Exception {
        if (args.length == 0) { help(); return; }
        if (!Files.exists(DB)) Files.createFile(DB);

        switch (args[0]) {
            case "add" -> add(String.join(" ", Arrays.copyOfRange(args,1,args.length)));
            case "list" -> list();
            case "done" -> done(Integer.parseInt(args[1]));
            case "clear" -> clear();
            default -> help();
        }
    }

    static List<Task> readAll() throws IOException {
        if (!Files.exists(DB)) return new ArrayList<>();
        return Files.readAllLines(DB).stream()
                .filter(s->!s.isBlank()).map(Task::fromCsv).collect(Collectors.toCollection(ArrayList::new));
    }
    static void writeAll(List<Task> tasks) throws IOException {
        Files.write(DB, tasks.stream().map(Task::toCsv).toList());
    }
    static void add(String text) throws IOException {
        var tasks = readAll();
        int nextId = tasks.stream().mapToInt(Task::id).max().orElse(0) + 1;
        tasks.add(new Task(nextId, text, false, LocalDateTime.now()));
        writeAll(tasks);
        System.out.println("Added #" + nextId + ": " + text);
    }
    static void list() throws IOException {
        var tasks = readAll();
        if(tasks.isEmpty()){ System.out.println("(empty)"); return; }
        tasks.forEach(t -> System.out.printf("%3d [%s] %s (at %s)%n", t.id(), t.done()?"x":" ", t.text(), t.createdAt()));
    }
    static void done(int id) throws IOException {
        var tasks = readAll();
        boolean ok=false;
        for (int i=0;i<tasks.size();i++){
            if(tasks.get(i).id()==id){ tasks.set(i, new Task(id, tasks.get(i).text(), true, tasks.get(i).createdAt())); ok=true; break; }
        }
        writeAll(tasks);
        System.out.println(ok?("Done #" + id):("Task #" + id + " not found"));
    }
    static void clear() throws IOException { writeAll(new ArrayList<>()); System.out.println("Cleared."); }
    static void help(){
        System.out.println("""
            Usage:
              mvn -q exec:java -Dexec.args="add Belajar Java"
              mvn -q exec:java -Dexec.args="list"
              mvn -q exec:java -Dexec.args="done 1"
              mvn -q exec:java -Dexec.args="clear"
            """);
    }
}
