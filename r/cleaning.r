# Basic cleaning: nama kolom, missing value, factor
clean_basic <- function(df) {
  suppressPackageStartupMessages({
    requireNamespace("dplyr", quietly = TRUE)
  })

  if (requireNamespace("janitor", quietly = TRUE)) {
    df <- janitor::clean_names(df)
  }

  df <- df |>
    dplyr::mutate(dplyr::across(where(is.character), ~trimws(.x)))

  na_ratio <- sapply(df, function(x) mean(is.na(x)))
  keep_cols <- names(na_ratio[na_ratio <= 0.30])
  df <- df[, intersect(names(df), keep_cols), drop = FALSE]

  mode_val <- function(v) {
    tab <- table(v, useNA = "no")
    if (length(tab) == 0) return(NA)
    names(tab)[which.max(tab)]
  }

  df <- df |>
    dplyr::mutate(
      dplyr::across(where(is.numeric), ~ ifelse(is.na(.x), stats::median(.x, na.rm = TRUE), .x)),
      dplyr::across(where(is.character), ~ ifelse(is.na(.x), mode_val(.x), .x))
    ) |>
    dplyr::mutate(dplyr::across(where(is.character), ~ if (dplyr::n_distinct(.x) <= 20) factor(.x) else .x))

  df
}
