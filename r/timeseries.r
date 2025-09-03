# Time series cepat: deteksi kolom tanggal & nilai, ARIMA + forecast 14 hari
run_timeseries <- function(df, date_col = NULL, value_col = NULL) {
  suppressPackageStartupMessages({
    requireNamespace("dplyr", quietly = TRUE)
    requireNamespace("lubridate", quietly = TRUE)
    requireNamespace("forecast", quietly = TRUE)
    requireNamespace("ggplot2", quietly = TRUE)
  })

  # Tebak kolom tanggal
  if (is.null(date_col)) {
    date_col <- names(df)[which(sapply(df, inherits, what = c("Date", "POSIXct", "POSIXt")))[1]]
    if (is.na(date_col)) {
      cand <- grep("date|tanggal|time|waktu", names(df), ignore.case = TRUE, value = TRUE)
      if (length(cand)) date_col <- cand[1]
    }
  }
  # Tebak kolom nilai
  if (is.null(value_col)) {
    num_cols <- names(dplyr::select(df, dplyr::where(is.numeric)))
    if (length(num_cols)) value_col <- num_cols[1]
  }
  if (is.null(date_col) || is.null(value_col)) {
    warning("Could not infer date/value columns for time series.")
    return(NULL)
  }

  df2 <- df |>
    dplyr::mutate(!!date_col := as.Date(.data[[date_col]])) |>
    dplyr::filter(!is.na(.data[[date_col]]), !is.na(.data[[value_col]])) |>
    dplyr::arrange(.data[[date_col]]) |>
    dplyr::group_by(.data[[date_col]]) |>
    dplyr::summarise(value = mean(.data[[value_col]], na.rm = TRUE), .groups = "drop")

  if (nrow(df2) < 10) {
    warning("Not enough data points for time series.")
    return(NULL)
  }

  ts_obj <- ts(df2$value, frequency = 7) # asumsi musiman mingguan untuk data harian
  fit <- forecast::auto.arima(ts_obj)
  fc <- forecast::forecast(fit, h = 14)

  fs::dir_create("outputs")
  p <- forecast::autoplot(fc)
  ggplot2::ggsave("outputs/ts_forecast.png", p, width = 7, height = 5)

  readr::write_csv(df2, "outputs/ts_series_prepared.csv")
  readr::write_csv(data.frame(point = as.numeric(fc$mean)), "outputs/ts_forecast_values.csv")

  list(model = fit, forecast = fc)
}
