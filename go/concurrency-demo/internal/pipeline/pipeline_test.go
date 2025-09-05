package pipeline

import (
	"context"
	"testing"
	"time"
)

func TestPipelineEndToEnd(t *testing.T) {
	ctx, cancel := context.WithTimeout(context.Background(), 2*time.Second)
	defer cancel()

	gen := Generator(ctx, 10)
	out := FanOut(ctx, gen, 3)         // 3 worker
	th := Throttle(ctx, out, 0)        // no throttle
	res := Collect(ctx, th)

	if len(res) != 10 {
		t.Fatalf("want 10 results, got %d", len(res))
	}
	for _, r := range res {
		if r.Err != nil {
			t.Fatalf("unexpected error: %v", r.Err)
		}
		if r.Output != r.Input*r.Input {
			t.Fatalf("bad output for %d: got %d", r.Input, r.Output)
		}
	}
}

func TestCancellation(t *testing.T) {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Millisecond)
	defer cancel()

	gen := Generator(ctx, 1_000_000) // besar, tapi kita cancel cepat
	out := FanOut(ctx, gen, 8)
	th := Throttle(ctx, out, 0)
	res := Collect(ctx, th)

	// tidak harus nol, tapi harus kurang dari total
	if len(res) >= 1_000_000 {
		t.Fatalf("expected early cancel, got %d", len(res))
	}
}
