export interface PagesResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  Items?: T[];
  TotalCount?: number;
  PageNumber?: number;
  PageSize?: number;
}

export interface SearchSortParams {
  search?: string;
  sortBy?: string;
  descending?: boolean;
  pageNumber: number;
  pageSize: number;
  email?: string
}
