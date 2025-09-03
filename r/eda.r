# Quick EDA: summary + plot otomatis
run_eda <- function(df, target = NULL, max_plots = 20) {
  suppressPackageStartupMessages({
    requireNamespace("dplyr", quietly = TRUE)
    requireNamespace("ggplot2", quietly = TRUE)
    requireNamespace("skimr", quietly = TRUE)
  })

  fs::dir_create("outputs")

  skim_tbl <- skimr::skim(df)
  readr::write_csv(as.data.frame(skim_tbl), file = "outputs/summary_skim.csv")

  num_df <- dplyr::select(df, dplyr::where(is.numeric))
  if (ncol(num_df) >= 2) {
    cmat <- suppressWarnings(stats::cor(num_df, use = "pairwise.complete.obs"))
    readr::write_csv(as.data.frame(cmat), "outputs/correlation_matrix.csv")
  }

  vars <- names(df)
  vars <- vars[seq_len(min(length(vars), max_plots))]
  for (v in vars) {
    p <- NULL
    if (is.numeric(df[[v]])) {
      p <- ggplot2::ggplot(df, ggplot2::aes(x = .data[[v]])) +
        ggplot2::geom_histogram(bins = 30) +
        ggplot2::labs(title = paste("Histogram of", v))
    } else if (is.factor(df[[v]]) || is.character(df[[v]])) {
      p <- ggplot2::ggplot(df, ggplot2::aes(x = .data[[v]])) +
        ggplot2::geom_bar() +
        ggplot2::coord_flip() +
        ggplot2::labs(title = paste("Bar plot of", v))
    }
    if (!is.null(p)) ggplot2::ggsave(filename = file.path("outputs", paste0("plot_", v, ".png")), p, width = 7, height = 5)
  }
}
