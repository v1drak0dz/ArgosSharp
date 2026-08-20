output "job_execution_queue_url" {
  value = aws_sqs_queue.job_executions.url
}

output "job_execution_queue_arn" {
  value = aws_sqs_queue.job_executions.arn
}

output "job_execution_dlq_url" {
  value = aws_sqs_queue.job_executions_dlq.url
}

