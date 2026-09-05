import { Directive, Input, TemplateRef, ViewContainerRef, inject, effect } from '@angular/core';
import { AuthStore } from '../../core/auth/auth.store';

@Directive({
  selector: '[hasPermission]',
  standalone: true
})
export class HasPermissionDirective {
  readonly #templateRef = inject(TemplateRef<unknown>);
  readonly #viewContainer = inject(ViewContainerRef);
  readonly #authStore = inject(AuthStore);

  #requiredPermissions: string[] = [];
  #hasView = false;

  @Input()
  set hasPermission(val: string | string[]) {
    this.#requiredPermissions = Array.isArray(val) ? val : [val];
    this.#updateView();
  }

  constructor() {
    // React to permission changes in AuthStore
    effect(() => {
      // Access the signal to create reactive dependency
      this.#authStore.permissions();
      this.#updateView();
    });
  }

  #updateView(): void {
    if (!this.#requiredPermissions.length) {
      if (!this.#hasView) {
        this.#viewContainer.createEmbeddedView(this.#templateRef);
        this.#hasView = true;
      }
      return;
    }

    const hasAccess = this.#requiredPermissions.some(perm => this.#authStore.hasPermission(perm));

    if (hasAccess && !this.#hasView) {
      this.#viewContainer.createEmbeddedView(this.#templateRef);
      this.#hasView = true;
    } else if (!hasAccess && this.#hasView) {
      this.#viewContainer.clear();
      this.#hasView = false;
    }
  }
}
