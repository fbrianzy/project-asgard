package httpx

import (
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"net/http"
	"strings"

	"https://github.com/fbrianzy/project-asgard/tree/main/go/rest-api/internal/store" // change the path
)

type Handlers struct {
	Users *store.MemoryUserStore
}

func NewHandlers(users *store.MemoryUserStore) *Handlers {
	return &Handlers{Users: users}
}

func writeJSON(w http.ResponseWriter, code int, v any) {
	w.Header().Set("Content-Type", "application/json; charset=utf-8")
	w.WriteHeader(code)
	_ = json.NewEncoder(w).Encode(v)
}

// GET /health
func (h *Handlers) Health(w http.ResponseWriter, r *http.Request) {
	writeJSON(w, http.StatusOK, map[string]any{
		"status": "ok",
	})
}

// GET /hello?name=Bagus
func (h *Handlers) Hello(w http.ResponseWriter, r *http.Request) {
	name := r.URL.Query().Get("name")
	if strings.TrimSpace(name) == "" {
		name = "there"
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": fmt.Sprintf("Hello, %s!", name)})
}

// -------- Users (simple path param parsing) --------

// POST /users   body: {"name":"...", "email":"..."}
func (h *Handlers) UsersCreate(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "method not allowed", http.StatusMethodNotAllowed)
		return
	}
	var payload struct {
		Name  string `json:"name"`
		Email string `json:"email"`
	}
	if err := json.NewDecoder(r.Body).Decode(&payload); err != nil {
		http.Error(w, "invalid json", http.StatusBadRequest)
		return
	}
	u, err := h.Users.Create(payload.Name, payload.Email)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}
	writeJSON(w, http.StatusCreated, u)
}

// GET /users
func (h *Handlers) UsersList(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		http.Error(w, "method not allowed", http.StatusMethodNotAllowed)
		return
	}
	users, _ := h.Users.List()
	writeJSON(w, http.StatusOK, users)
}

// GET /users/{id}
func (h *Handlers) UsersGet(w http.ResponseWriter, r *http.Request) {
	id, ok := takeIDFromPath("/users/", r.URL.Path)
	if !ok {
		http.NotFound(w, r)
		return
	}
	u, err := h.Users.Get(id)
	if err != nil {
		status := http.StatusInternalServerError
		if errors.Is(err, store.ErrNotFound) {
			status = http.StatusNotFound
		}
		http.Error(w, err.Error(), status)
		return
	}
	writeJSON(w, http.StatusOK, u)
}

// PUT /users/{id}  body: {"name":"...", "email":"..."}
func (h *Handlers) UsersUpdate(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPut {
		http.Error(w, "method not allowed", http.StatusMethodNotAllowed)
		return
	}
	id, ok := takeIDFromPath("/users/", r.URL.Path)
	if !ok {
		http.NotFound(w, r)
		return
	}
	data, err := io.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "cannot read body", http.StatusBadRequest)
		return
	}
	var payload struct {
		Name  string `json:"name"`
		Email string `json:"email"`
	}
	if err := json.Unmarshal(data, &payload); err != nil {
		http.Error(w, "invalid json", http.StatusBadRequest)
		return
	}
	u, err := h.Users.Update(id, payload.Name, payload.Email)
	if err != nil {
		status := http.StatusBadRequest
		if errors.Is(err, store.ErrNotFound) {
			status = http.StatusNotFound
		}
		http.Error(w, err.Error(), status)
		return
	}
	writeJSON(w, http.StatusOK, u)
}

// DELETE /users/{id}
func (h *Handlers) UsersDelete(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodDelete {
		http.Error(w, "method not allowed", http.StatusMethodNotAllowed)
		return
	}
	id, ok := takeIDFromPath("/users/", r.URL.Path)
	if !ok {
		http.NotFound(w, r)
		return
	}
	if err := h.Users.Delete(id); err != nil {
		status := http.StatusInternalServerError
		if errors.Is(err, store.ErrNotFound) {
			status = http.StatusNotFound
		}
		http.Error(w, err.Error(), status)
		return
	}
	w.WriteHeader(http.StatusNoContent)
}

func takeIDFromPath(prefix, path string) (string, bool) {
	if !strings.HasPrefix(path, prefix) {
		return "", false
	}
	id := strings.TrimPrefix(path, prefix)
	if id == "" || strings.Contains(id, "/") {
		return "", false
	}
	return id, true
}
