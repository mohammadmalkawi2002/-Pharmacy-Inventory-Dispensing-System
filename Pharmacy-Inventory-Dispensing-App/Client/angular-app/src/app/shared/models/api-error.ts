/**
 * Matches .NET ProblemDetails response shape.
 */
export interface ApiError {
  type?: string;
  title: string;
  status: number;
  detail?: string;
  errors?: Record<string, string[]>;
}
