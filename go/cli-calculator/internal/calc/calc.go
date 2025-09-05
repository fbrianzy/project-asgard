package calc

import (
	"errors"
	"math"
)

var (
	ErrEmptyInput   = errors.New("no numbers provided")
	ErrDivideByZero = errors.New("division by zero")
	ErrUnknownOp    = errors.New("unknown operation")
)

type Op string

const (
	Add Op = "add"
	Sub Op = "sub"
	Mul Op = "mul"
	Div Op = "div"
	Avg Op = "avg"
	Min Op = "min"
	Max Op = "max"
)

// Compute menjalankan operasi pada slice angka.
// Aturan:
// - add, mul: boleh 1+ angka
// - sub, div: minimal 2 angka (left-associative)
// - avg, min, max: minimal 1 angka
func Compute(op Op, nums []float64) (float64, error) {
	if len(nums) == 0 {
		return 0, ErrEmptyInput
	}

	switch op {
	case Add:
		var sum float64
		for _, n := range nums {
			sum += n
		}
		return sum, nil

	case Sub:
		if len(nums) < 2 {
			return 0, errors.New("sub requires at least 2 numbers")
		}
		res := nums[0]
		for _, n := range nums[1:] {
			res -= n
		}
		return res, nil

	case Mul:
		res := 1.0
		for _, n := range nums {
			res *= n
		}
		return res, nil

	case Div:
		if len(nums) < 2 {
			return 0, errors.New("div requires at least 2 numbers")
		}
		res := nums[0]
		for _, n := range nums[1:] {
			if n == 0 {
				return 0, ErrDivideByZero
			}
			res /= n
		}
		return res, nil

	case Avg:
		var sum float64
		for _, n := range nums {
			sum += n
		}
		return sum / float64(len(nums)), nil

	case Min:
		m := math.Inf(1)
		for _, n := range nums {
			if n < m {
				m = n
			}
		}
		return m, nil

	case Max:
		m := math.Inf(-1)
		for _, n := range nums {
			if n > m {
				m = n
			}
		}
		return m, nil

	default:
		return 0, ErrUnknownOp
	}
}
