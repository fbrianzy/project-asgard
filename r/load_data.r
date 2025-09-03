# Load dataset (CSV/XLSX/SAV/DTA/JSON) atau buat dummy
load_data <- function(path = NULL, sheet = NULL) {
  suppressPackageStartupMessages({
    requireNamespace("readr", quietly = TRUE)
    requireNamespace("tibble", quietly = TRUE)
    requireNamespace("dplyr", quietly = TRUE)
  })

  fs::dir_create("outputs")
  fs::dir_create("data")

  if (is.null(path) || !nzchar(path)) {
    message("No path provided. Generating dummy dataset: data/dummy.csv")
    set.seed(42)
    n <- 500
    df <- tibble::tibble(
      id = seq_len(n),
      date = as.Date("2024-01-01") + sample(0:400, n, replace = TRUE),
      gender = sample(c("Male", "Female"), n, replace = TRUE),
      city = sample(c("Surabaya", "Sidoarjo", "Malang", "Gresik"), n, replace = TRUE),
      age = pmin(pmax(round(rnorm(n, 30, 8)), 18), 60),
      income = round(exp(rnorm(n, log(5e6), 0.6))),
      spend = round(runif(n, 1e5, 5e6)),
      churn = rbinom(n, 1, prob = 0.22)
    ) |>
      dplyr::mutate(
        spend_rate = spend / pmax(income, 1),
        group = dplyr::case_when(
          spend_rate < 0.05 ~ "Low",
          spend_rate < 0.15 ~ "Medium",
          TRUE ~ "High"
        )
      )
    readr::write_csv(df, "data/dummy.csv")
    return(df)
  }

  stopifnot(file.exists(path))
  ext <- tolower(fs::path_ext(path))

  if (!requireNamespace("rio", quietly = TRUE)) install.packages("rio", quiet = TRUE)
  if (!requireNamespace("readxl", quietly = TRUE)) install.packages("readxl", quiet = TRUE)

  if (ext %in% c("csv")) {
    return(readr::read_csv(path, show_col_types = FALSE))
  } else if (ext %in% c("xlsx", "xls")) {
    return(readxl::read_excel(path, sheet = sheet))
  } else if (ext %in% c("sav", "dta", "json", "tsv", "rds")) {
    return(rio::import(path))
  } else {
    stop("Unsupported file type: ", ext)
  }
}
