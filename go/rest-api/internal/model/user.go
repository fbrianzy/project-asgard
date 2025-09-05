package model

import (
	"errors"
	"strings"
	"time"
)

var (
	ErrInvalidName  = errors.New("name must not be empty")
	ErrInvalidEmail = errors.New("invalid email")
)

type User struct {
	ID        string    `json:"id"`
	Name      string    `json:"name"`
	Email     string    `json:"email"`
	CreatedAt time.Time `json:"created_at"`
	UpdatedAt time.Time `json:"updated_at"`
}

func (u *User) Validate() error {
	if strings.TrimSpace(u.Name) == "" {
		return ErrInvalidName
	}
	// super simple email check (cukup untuk demo)
	if !strings.Contains(u.Email, "@") || len(u.Email) < 5 {
		return ErrInvalidEmail
	}
	return nil
}
