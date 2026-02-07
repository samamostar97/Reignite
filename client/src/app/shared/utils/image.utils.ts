import { environment } from '../../../environments/environment';

/**
 * Converts a relative image path to a full URL.
 * If the path is already absolute (http/https), returns it as-is.
 * Returns empty string if path is null/undefined.
 */
export function getImageUrl(path: string | null | undefined): string {
  if (!path) return '';
  if (path.startsWith('http://') || path.startsWith('https://')) {
    return path;
  }
  return `${environment.baseUrl}${path}`;
}

/**
 * Generates initials from first and last name.
 * Returns uppercase initials (e.g., "JD" for "John Doe").
 */
export function getInitials(firstName: string, lastName: string): string {
  const first = firstName?.charAt(0) || '';
  const last = lastName?.charAt(0) || '';
  return `${first}${last}`.toUpperCase();
}
