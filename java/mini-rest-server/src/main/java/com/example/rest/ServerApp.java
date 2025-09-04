package com.example.rest;

import com.sun.net.httpserver.*;
import java.io.IOException;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.nio.charset.StandardCharsets;
import java.util.*;

public class ServerApp {
    static final List<Map<String,Object>> TODOS = new ArrayList<>();

    public static void main(String[] args) throws IOException {
        int port = args.length>0 ? Integer.parseInt(args[0]) : 8080;
        HttpServer server = HttpServer.create(new InetSocketAddress(port), 0);

        server.createContext("/health", ex -> write(ex, 200, "{\"status\":\"ok\"}"));
        server.createContext("/todos", ex -> {
            switch (ex.getRequestMethod()) {
                case "GET" -> write(ex, 200, json(TODOS));
                case "POST" -> {
                    String query = Optional.ofNullable(ex.getRequestURI().getQuery()).orElse("");
                    String title = Arrays.stream(query.split("&"))
                            .map(s->s.split("=",2))
                            .filter(p->p.length==2 && p[0].equals("title"))
                            .map(p->java.net.URLDecoder.decode(p[1], StandardCharsets.UTF_8))
                            .findFirst().orElse("Untitled");
                    Map<String,Object> item = new LinkedHashMap<>();
                    item.put("id", TODOS.size()+1);
                    item.put("title", title);
                    item.put("done", false);
                    TODOS.add(item);
                    write(ex, 201, json(item));
                }
                default -> write(ex, 405, "{\"error\":\"method not allowed\"}");
            }
        });

        server.setExecutor(null);
        server.start();
        System.out.println("Server running on http://localhost:"+port);
        System.out.println("Endpoints: /health, /todos [GET|POST?title=...]");
    }

    static String json(Object o){
        if(o instanceof List<?> list){
            StringBuilder sb=new StringBuilder("[");
            for(int i=0;i<list.size();i++){ if(i>0) sb.append(","); sb.append(json(list.get(i))); }
            return sb.append("]").toString();
        }
        if(o instanceof Map<?,?> m){
            StringBuilder sb=new StringBuilder("{"); int i=0;
            for(var e: m.entrySet()){
                if(i++>0) sb.append(",");
                sb.append("\"").append(e.getKey()).append("\":");
                Object v = e.getValue();
                if(v instanceof Number || v instanceof Boolean) sb.append(v);
                else sb.append("\"").append(v.toString().replace("\"","\\\"")).append("\"");
            }
            return sb.append("}").toString();
        }
        return "\""+o+"\"";
    }

    static void write(HttpExchange ex, int code, String body) throws IOException {
        byte[] b = body.getBytes(StandardCharsets.UTF_8);
        ex.getResponseHeaders().add("Content-Type","application/json; charset=utf-8");
        ex.sendResponseHeaders(code, b.length);
        try(OutputStream os = ex.getResponseBody()){ os.write(b); }
    }
}
