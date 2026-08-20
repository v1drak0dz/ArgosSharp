resource "aws_sqs_queue" "job_executions" {
  name = "argos-job-executions"

  visibility_timeout_seconds = 300

  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.job_executions_dlq.arn
    maxReceiveCount     = 3
  })
}

resource "aws_sqs_queue" "job_executions_dlq" {
  name = "argos-job-executions-dlq"
}

resource "aws_s3_bucket" "artifacts" {
  bucket = "argos-artifacts"
}
