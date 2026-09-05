/**
 * Matches the backend paginated response shape.
 */
export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

/**
 * Query parameters sent to paginated backend endpoints.
 */
export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  [key: string]: string | number | boolean | undefined;
}
