package main

import (
	"context"
	"log"
	"net/http"
	"os"
	"os/signal"
	"time"

	"https://github.com/fbrianzy/project-asgard/tree/main/go/rest-api/internal/httpx" // change the path
	"https://github.com/fbrianzy/project-asgard/tree/main/go/rest-api/internal/store" // change the path
)

func main() {
	// PORT via env (default 8080)
	port := os.Getenv("PORT")
	if port == "" {
		port = "8080"
	}

	users := store.NewMemoryUserStore()
	h := httpx.NewHandlers(users)

	// Router + routes
	r := httpx.NewRouter()
	r.Handle("/health", http.HandlerFunc(h.Health))
	r.Handle("/hello", http.HandlerFunc(h.Hello))

	// Users collection
	r.Handle("/users", httpx.Logging(httpx.Recoverer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		switch r.Method {
		case http.MethodGet:
			h.UsersList(w, r)
		case http.MethodPost:
			h.UsersCreate(w, r)
		default:
			http.Error(w, "method not allowed", http.StatusMethodNotAllowed)
		}
	}))))

	// Users item
	r.Handle("/users/", httpx.Logging(httpx.Recoverer(http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		switch r.Method {
		case http.MethodGet:
			h.UsersGet(w, r)
		case http.MethodPut:
			h.UsersUpdate(w, r)
		case http.MethodDelete:
			h.UsersDelete(w, r)
		default:
			http.Error(w, "method not allowed", http.StatusMethodNotAllowed)
		}
	}))))

	// Health & Hello tanpa middleware berat
	mux := http.NewServeMux()
	mux.Handle("/health", http.HandlerFunc(h.Health))
	mux.Handle("/hello", http.HandlerFunc(h.Hello))
	mux.Handle("/users", r.Handler())
	mux.Handle("/users/", r.Handler())

	srv := &http.Server{
		Addr:         ":" + port,
		Handler:      mux,
		ReadTimeout:  5 * time.Second,
		WriteTimeout: 10 * time.Second,
		IdleTimeout:  60 * time.Second,
	}

	// Graceful shutdown
	go func() {
		log.Printf("REST API listening on http://localhost:%s", port)
		if err := srv.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			log.Fatalf("server error: %v", err)
		}
	}()

	// Wait for CTRL+C
	stop := make(chan os.Signal, 1)
	signal.Notify(stop, os.Interrupt)
	<-stop
	log.Println("shutting down...")

	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	if err := srv.Shutdown(ctx); err != nil {
		log.Fatalf("graceful shutdown failed: %v", err)
	}
	log.Println("server stopped")
}
