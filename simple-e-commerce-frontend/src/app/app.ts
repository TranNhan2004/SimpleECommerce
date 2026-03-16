import { Component, computed, effect, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterOutlet } from '@angular/router';
import { TranslocoService } from '@jsverse/transloco';

import {
  LanguageTranslationKey,
  MetaTranslationKey,
  NavigationTranslationKey,
  TranslationKey
} from './core/i18n/translation-keys';
import { AppConstants } from './core/constants';

type AppLanguage = 'en' | 'vi';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly transloco = inject(TranslocoService);
  private readonly activeTranslation = toSignal(this.transloco.selectTranslation(), { initialValue: {} });

  protected readonly title = signal('Simple');
  protected readonly currentLanguage = signal<AppLanguage>(this.resolveInitialLanguage());
  protected readonly isAuthenticated = signal(false);

  protected readonly languages = [
    { code: 'en', label: 'EN', flagPath: '/flags/en.svg', ariaKey: 'language.switchToEnglish' },
    { code: 'vi', label: 'VI', flagPath: '/flags/vi.svg', ariaKey: 'language.switchToVietnamese' }
  ] as const satisfies readonly {
    code: AppLanguage;
    label: string;
    flagPath: string;
    ariaKey: LanguageTranslationKey;
  }[];

  protected readonly navLinks = [
    { labelKey: 'nav.categories', icon: 'grid_view' },
    { labelKey: 'nav.newArrivals', icon: 'auto_awesome' },
    { labelKey: 'nav.bestSellers', icon: 'workspace_premium' },
    { labelKey: 'nav.flashDeals', icon: 'local_fire_department' },
    { labelKey: 'nav.support', icon: 'support_agent' }
  ] as const satisfies readonly {
    labelKey: NavigationTranslationKey;
    icon: string;
  }[];

  protected readonly navMeta = [
    { labelKey: 'meta.freeShipping', icon: 'local_shipping' },
    { labelKey: 'meta.returns', icon: 'autorenew' },
    { labelKey: 'meta.secureCheckout', icon: 'verified_user' }
  ] as const satisfies readonly {
    labelKey: MetaTranslationKey;
    icon: string;
  }[];

  protected readonly accountLabelKey = computed<NavigationTranslationKey>(() =>
    this.isAuthenticated() ? 'nav.account' : 'nav.login'
  );

  constructor() {
    const language = this.currentLanguage();
    this.transloco.setActiveLang(language);

    effect(() => {
      const currentLanguage = this.currentLanguage();
      this.transloco.setActiveLang(currentLanguage);
      localStorage.setItem(AppConstants.LANGUAGE_STORAGE_KEY, currentLanguage);
    });
  }

  protected switchLanguage(language: AppLanguage): void {
    if (this.currentLanguage() === language) {
      return;
    }

    this.currentLanguage.set(language);
  }

  protected t(key: TranslationKey): string {
    this.activeTranslation();
    return this.transloco.translate(key);
  }

  private normalizeLanguage(language: string | null | undefined): AppLanguage {
    return language === 'vi' ? 'vi' : 'en';
  }

  private resolveInitialLanguage(): AppLanguage {
    const storedLanguage = localStorage.getItem(AppConstants.LANGUAGE_STORAGE_KEY);

    if (storedLanguage === 'en' || storedLanguage === 'vi') {
      return storedLanguage;
    }

    return this.normalizeLanguage(this.transloco.getActiveLang());
  }
}
