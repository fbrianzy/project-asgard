package calc

import "testing"

func almostEqual(a, b float64) bool {
	const eps = 1e-9
	diff := a - b
	if diff < 0 {
		diff = -diff
	}
	return diff < eps
}

func TestAdd(t *testing.T) {
	got, err := Compute(Add, []float64{1, 2, 3})
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if !almostEqual(got, 6) {
		t.Fatalf("want 6, got %v", got)
	}
}

func TestSub(t *testing.T) {
	got, err := Compute(Sub, []float64{10, 2, 3})
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if !almostEqual(got, 5) {
		t.Fatalf("want 5, got %v", got)
	}
}

func TestMul(t *testing.T) {
	got, err := Compute(Mul, []float64{2, 3, 4})
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if !almostEqual(got, 24) {
		t.Fatalf("want 24, got %v", got)
	}
}

func TestDiv(t *testing.T) {
	got, err := Compute(Div, []float64{20, 2, 2})
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if !almostEqual(got, 5) {
		t.Fatalf("want 5, got %v", got)
	}
}

func TestAvg(t *testing.T) {
	got, err := Compute(Avg, []float64{2, 4, 6, 8})
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if !almostEqual(got, 5) {
		t.Fatalf("want 5, got %v", got)
	}
}

func TestMinMax(t *testing.T) {
	min, err := Compute(Min, []float64{5, -1, 3})
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if !almostEqual(min, -1) {
		t.Fatalf("want -1, got %v", min)
	}

	max, err := Compute(Max, []float64{5, -1, 3})
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	if !almostEqual(max, 5) {
		t.Fatalf("want 5, got %v", max)
	}
}
