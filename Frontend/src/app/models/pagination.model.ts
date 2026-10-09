export interface Pagination<T> {
  pageIndex: number;
  pageSize: number;
  count: number;
  totalPages: number;
  data: T[];
}
