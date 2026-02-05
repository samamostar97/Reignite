import {
  trigger,
  transition,
  style,
  animate,
  query
} from '@angular/animations';

export const routeAnimations = trigger('routeAnimations', [
  transition('* <=> *', [
    query(':enter', [
      style({ opacity: 0 })
    ], { optional: true }),

    query(':leave', [
      animate('150ms ease-out', style({ opacity: 0 }))
    ], { optional: true }),

    query(':enter', [
      animate('200ms ease-in', style({ opacity: 1 }))
    ], { optional: true })
  ])
]);
