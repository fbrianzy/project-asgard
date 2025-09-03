# Klasifikasi biner: logistic vs random forest (caret)
run_classification <- function(df, target) {
  suppressPackageStartupMessages({
    requireNamespace("dplyr", quietly = TRUE)
    requireNamespace("caret", quietly = TRUE)
    requireNamespace("pROC", quietly = TRUE)
    requireNamespace("randomForest", quietly = TRUE)
  })

  stopifnot(target %in% names(df))
  df <- dplyr::select(df, dplyr::all_of(target), dplyr::where(function(x) is.numeric(x) || is.factor(x)))

  # pastikan target biner factor
  if (is.numeric(df[[target]])) df[[target]] <- factor(df[[target]] > median(df[[target]], na.rm = TRUE), labels = c("Low","High"))
  if (!is.factor(df[[target]])) df[[target]] <- factor(df[[target]])
  if (length(levels(df[[target]])) != 2) {
    warning("Target is not binary after processing.")
    return(NULL)
  }

  set.seed(1)
  tr <- caret::trainControl(
    method = "repeatedcv", number = 5, repeats = 2,
    classProbs = TRUE, summaryFunction = caret::twoClassSummary
  )

  # Logistic Regression
  logit <- caret::train(
    as.formula(paste(target, "~ .")),
    data = df, method = "glm", family = binomial(),
    metric = "ROC", trControl = tr
  )

  # Random Forest
  rf <- caret::train(
    as.formula(paste(target, "~ .")),
    data = df, method = "rf",
    metric = "ROC", trControl = tr
  )

  fs::dir_create("outputs")
  res <- dplyr::bind_rows(
    data.frame(model = "logistic", ROC = max(logit$results$ROC, na.rm = TRUE)),
    data.frame(model = "random_forest", ROC = max(rf$results$ROC, na.rm = TRUE))
  )
  readr::write_csv(res, "outputs/classification_roc.csv")

  list(logistic = logit, random_forest = rf)
}
