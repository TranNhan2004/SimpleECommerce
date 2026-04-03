import { effect, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TranslocoService } from '@jsverse/transloco';
import { AppConstants } from '../../core/constants';
import { Language } from '../../core/enums/language';

export abstract class Translation {
  protected readonly transloco = inject(TranslocoService);
  protected readonly currentLanguage = signal<Language>(this.resolveInitialLanguage());
  private readonly activeTranslation = toSignal(this.transloco.selectTranslation(), { initialValue: {} });

  constructor() {
    this.transloco.setActiveLang(this.currentLanguage());

    effect(() => {
      const language = this.currentLanguage();
      this.transloco.setActiveLang(language);
      localStorage.setItem(AppConstants.LANGUAGE_STORAGE_KEY, language);
    });
  }

  protected t(key: string): string {
    const translation = this.activeTranslation();
    if (!translation || Object.keys(translation).length === 0) {
      return '';
    }

    return this.transloco.translate(key);
  }

  protected switchLanguage(language: Language): void {
    if (this.currentLanguage() === language) {
      return;
    }

    this.currentLanguage.set(language);
  }

  private normalizeLanguage(language: string | null | undefined): Language {
    return language?.trim().toLocaleLowerCase() === 'vi' ? Language.Vietnamese : Language.English;
  }

  private resolveInitialLanguage(): Language {
    const storedLanguage = localStorage.getItem(AppConstants.LANGUAGE_STORAGE_KEY);

    if (storedLanguage === 'en' || storedLanguage === 'vi') {
      return this.normalizeLanguage(storedLanguage);
    }

    return this.normalizeLanguage(this.transloco.getActiveLang());
  }
}
