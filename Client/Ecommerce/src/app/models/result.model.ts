export interface Result<T> {
  isSuccess: boolean;
  errorMessages?: string[];
  data: T;
}
