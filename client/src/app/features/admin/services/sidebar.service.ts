import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SidebarService {
  private readonly _isCollapsed = signal(false);

  readonly isCollapsed = this._isCollapsed.asReadonly();

  toggle(): void {
    this._isCollapsed.update(v => !v);
  }

  collapse(): void {
    this._isCollapsed.set(true);
  }

  expand(): void {
    this._isCollapsed.set(false);
  }
}
