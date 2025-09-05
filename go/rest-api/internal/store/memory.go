package store

import (
	"errors"
	"sync"
	"time"

	"https://github.com/fbrianzy/project-asgard/tree/main/go/rest-ap/internal/model" // change the path
)

var (
	ErrNotFound = errors.New("user not found")
)

type MemoryUserStore struct {
	mu    sync.RWMutex
	seq   int64
	items map[string]*model.User
}

func NewMemoryUserStore() *MemoryUserStore {
	return &MemoryUserStore{
		items: make(map[string]*model.User),
	}
}

func (s *MemoryUserStore) nextID() string {
	s.seq++
	return time.Now().UTC().Format("20060102") + "-" + pad6(s.seq)
}

func pad6(n int64) string {
	// zero-pad 6 digits (demo ID rapi)
	const width = 6
	d := []byte{'0','0','0','0','0','0'}
	i := len(d) - 1
	for n > 0 && i >= 0 {
		d[i] = byte('0' + (n % 10))
		n /= 10
		i--
	}
	return string(d)
}

// Create
func (s *MemoryUserStore) Create(name, email string) (*model.User, error) {
	s.mu.Lock()
	defer s.mu.Unlock()

	u := &model.User{
		ID:        s.nextID(),
		Name:      name,
		Email:     email,
		CreatedAt: time.Now().UTC(),
		UpdatedAt: time.Now().UTC(),
	}
	if err := u.Validate(); err != nil {
		return nil, err
	}
	s.items[u.ID] = u
	return u, nil
}

// List
func (s *MemoryUserStore) List() ([]*model.User, error) {
	s.mu.RLock()
	defer s.mu.RUnlock()

	out := make([]*model.User, 0, len(s.items))
	for _, u := range s.items {
		out = append(out, u)
	}
	return out, nil
}

// Get
func (s *MemoryUserStore) Get(id string) (*model.User, error) {
	s.mu.RLock()
	defer s.mu.RUnlock()

	u, ok := s.items[id]
	if !ok {
		return nil, ErrNotFound
	}
	return u, nil
}

// Update
func (s *MemoryUserStore) Update(id, name, email string) (*model.User, error) {
	s.mu.Lock()
	defer s.mu.Unlock()

	u, ok := s.items[id]
	if !ok {
		return nil, ErrNotFound
	}
	if name != "" {
		u.Name = name
	}
	if email != "" {
		u.Email = email
	}
	if err := u.Validate(); err != nil {
		return nil, err
	}
	u.UpdatedAt = time.Now().UTC()
	return u, nil
}

// Delete
func (s *MemoryUserStore) Delete(id string) error {
	s.mu.Lock()
	defer s.mu.Unlock()

	if _, ok := s.items[id]; !ok {
		return ErrNotFound
	}
	delete(s.items, id)
	return nil
}
