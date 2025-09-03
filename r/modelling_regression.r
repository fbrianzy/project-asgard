# Regresi linear + stepAIC + diagnostics
run_regression <- function(df, target) {
  suppressPackageStartupMessages({
    requireNamespace("dplyr", quietly = TRUE)
    requireNamespace("ggplot2", quietly = TRUE)
    requireNamespace("broom", quietly = TRUE)
    requireNamespace("MASS", quietly = TRUE)
  })

  stopifnot(target %in% names(df))

  num_cols <- names(dplyr::select(df, dplyr::where(is.numeric)))
  num_cols <- setdiff(num_cols, target)
  if (length(num_cols) == 0) {
    warning("No numeric predictors available for regression.")
    return(NULL)
  }

  form <- as.formula(paste(target, "~", paste(num_cols, collapse = "+")))
  m0 <- stats::lm(form, data = df)
  m_step <- MASS::stepAIC(m0, trace = FALSE)

  fs::dir_create("outputs")
  readr::write_csv(broom::tidy(m_step), "outputs/regression_coefficients.csv")
  readr::write_csv(broom::glance(m_step), "outputs/regression_metrics.csv")

  p <- ggplot2::ggplot(
    data.frame(fitted = fitted(m_step), resid = resid(m_step)),
    ggplot2::aes(fitted, resid)
  ) +
    ggplot2::geom_point(alpha = 0.6) +
    ggplot2::geom_hline(yintercept = 0, linetype = 2) +
    ggplot2::labs(title = "Residuals vs Fitted")
  ggplot2::ggsave("outputs/regression_residuals.png", p, width = 7, height = 5)

  m_step
}
