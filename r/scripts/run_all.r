# Jalankan seluruh pipeline dari CLI.
# Contoh:
#   Rscript scripts/run_all.R
#   Rscript scripts/run_all.R data/myfile.csv target=spend cls_target=churn date=date value=spend

suppressPackageStartupMessages({
  if (!requireNamespace("pacman", quietly = TRUE)) install.packages("pacman", quiet = TRUE)
  pacman::p_load(
    fs, readr, dplyr, ggplot2, skimr, janitor, readxl, rio, broom, MASS,
    caret, pROC, randomForest, lubridate, forecast
  )
})

source("R/load_data.R")
source("R/cleaning.R")
source("R/eda.R")
source("R/modeling_regression.R")
source("R/modeling_classification.R")
source("R/timeseries.R")

args <- commandArgs(trailingOnly = TRUE)
path <- ifelse(length(args) >= 1 && !grepl("=", args[1]), args[1], "")
get_arg <- function(key, default = NULL) {
  hit <- grep(paste0("^", key, "="), args, value = TRUE)
  if (length(hit)) sub(paste0("^", key, "="), "", hit[1]) else default
}

message("\n=== STATS STARTER ===")
message("Data path: ", ifelse(nzchar(path), path, "<dummy>"))

DF <- load_data(path)
DF <- clean_basic(DF)

# EDA
run_eda(DF, target = get_arg("target"))

# Regresi (jika target numerik)
reg_target <- get_arg("target")
if (!is.null(reg_target) && reg_target %in% names(DF) && is.numeric(DF[[reg_target]])) {
  message("Fitting regression on target=", reg_target)
  run_regression(DF, reg_target)
}

# Klasifikasi (jika cls_target tersedia)
cls_target <- get_arg("cls_target")
if (!is.null(cls_target) && cls_target %in% names(DF)) {
  message("Fitting classification on target=", cls_target)
  run_classification(DF, cls_target)
}

# Time series (pakai argumen atau auto-infer)
run_timeseries(DF, date_col = get_arg("date"), value_col = get_arg("value"))

message("\nAll done. Check the outputs/ folder.")
