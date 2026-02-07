import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SidebarService {
  private readonly _isCollapsed = signal(false);
  private readonly _isMobileOpen = signal(false);

  readonly isCollapsed = this._isCollapsed.asReadonly();
  readonly isMobileOpen = this._isMobileOpen.asReadonly();

  toggle(): void {
    this._isCollapsed.update(v => !v);
  }

  collapse(): void {
    this._isCollapsed.set(true);
  }

  expand(): void {
    this._isCollapsed.set(false);
  }

  toggleMobile(): void {
    this._isMobileOpen.update(v => !v);
  }

  closeMobile(): void {
    this._isMobileOpen.set(false);
  }
}
