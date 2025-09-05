package pipeline

import (
	"context"
	"errors"
	"math"
	"math/rand"
	"sync"
	"time"
)

// Task adalah unit kerja sederhana berisi input (n) dan hasil.
type Task struct {
	ID     int
	Input  int
	Output int
	Err    error
	Spent  time.Duration
}

// Generator mengirim angka 1..count ke out. Stop kalau context selesai.
func Generator(ctx context.Context, count int) <-chan Task {
	out := make(chan Task)
	go func() {
		defer close(out)
		for i := 1; i <= count; i++ {
			select {
			case <-ctx.Done():
				return
			case out <- Task{ID: i, Input: i}:
			}
		}
	}()
	return out
}

// Worker melakukan kerja CPU-bound: menghitung kuadrat dengan sedikit jitter.
// Simulasi beban via sleep [1..5] ms. Menghormati context.
func Worker(ctx context.Context, in <-chan Task) <-chan Task {
	out := make(chan Task)
	go func() {
		defer close(out)
		r := rand.New(rand.NewSource(time.Now().UnixNano()))
		for t := range in {
			start := time.Now()

			// simulasi jitter
			select {
			case <-ctx.Done():
				return
			case <-time.After(time.Duration(r.Intn(5)+1) * time.Millisecond):
			}

			// logika utama
			if t.Input < 0 || t.Input > int(math.Sqrt(math.MaxInt/2)) {
				t.Err = errors.New("input out of range")
			} else {
				t.Output = t.Input * t.Input
			}
			t.Spent = time.Since(start)

			select {
			case <-ctx.Done():
				return
			case out <- t:
			}
		}
	}()
	return out
}

// FanOut membuat pool pekerja (nWorkers). Menggabungkan output via FanIn.
func FanOut(ctx context.Context, in <-chan Task, nWorkers int) <-chan Task {
	outs := make([]<-chan Task, 0, nWorkers)
	for i := 0; i < nWorkers; i++ {
		outs = append(outs, Worker(ctx, in))
	}
	return FanIn(ctx, outs...)
}

// FanIn menggabungkan banyak channel menjadi satu.
func FanIn(ctx context.Context, chans ...<-chan Task) <-chan Task {
	var wg sync.WaitGroup
	out := make(chan Task)

	output := func(c <-chan Task) {
		defer wg.Done()
		for t := range c {
			select {
			case <-ctx.Done():
				return
			case out <- t:
			}
		}
	}

	wg.Add(len(chans))
	for _, c := range chans {
		go output(c)
	}

	go func() {
		wg.Wait()
		close(out)
	}()
	return out
}

// Throttle membatasi laju konsumsi menggunakan ticker setiap 'every'.
func Throttle(ctx context.Context, in <-chan Task, every time.Duration) <-chan Task {
	out := make(chan Task)
	ticker := time.NewTicker(every)
	go func() {
		defer close(out)
		defer ticker.Stop()
		for t := range in {
			select {
			case <-ctx.Done():
				return
			case <-ticker.C:
				select {
				case <-ctx.Done():
					return
				case out <- t:
				}
			}
		}
	}()
	return out
}

// Collect mengumpulkan semua hasil ke slice.
func Collect(ctx context.Context, in <-chan Task) []Task {
	results := make([]Task, 0, 64)
	for {
		select {
		case <-ctx.Done():
			return results
		case t, ok := <-in:
			if !ok {
				return results
			}
			results = append(results, t)
		}
	}
}
