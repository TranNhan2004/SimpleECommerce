import { Component, computed, inject, signal } from '@angular/core';
import { LanguageTranslationKey, MetaTranslationKey, NavigationTranslationKey } from '../../core/i18n/translation-keys';
import { AuthService } from '../../core/services/auth.service';
import { Language } from '../../core/enums/language';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Translation } from '../translation/translation';

@Component({
  selector: 'app-navbar',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar extends Translation {
  protected readonly auth = inject(AuthService);

  protected readonly title = signal('Simple');
  protected readonly isAuthenticated = computed(() => this.auth.isAuthenticated());

  protected readonly languages = [
    { code: Language.English, label: 'EN', flagPath: '/flags/en.svg', ariaKey: 'language.switchToEnglish' },
    { code: Language.Vietnamese, label: 'VI', flagPath: '/flags/vi.svg', ariaKey: 'language.switchToVietnamese' }
  ] as const satisfies readonly {
    code: Language;
    label: string;
    flagPath: string;
    ariaKey: LanguageTranslationKey;
  }[];

  protected readonly navLinks = [
    { labelKey: 'nav.home', icon: 'home', route: '/' },
    { labelKey: 'nav.support', icon: 'support_agent', route: '/' }
  ] as const satisfies readonly {
    labelKey: NavigationTranslationKey;
    icon: string;
    route: string;
  }[];

  protected readonly navMeta = [
    { labelKey: 'meta.freeShipping', icon: 'local_shipping' },
    { labelKey: 'meta.returns', icon: 'autorenew' },
    { labelKey: 'meta.secureCheckout', icon: 'verified_user' }
  ] as const satisfies readonly {
    labelKey: MetaTranslationKey;
    icon: string;
  }[];

  protected readonly signedInAs = computed(() => this.auth.getDisplayName());

  protected get isAuthenticatedValue(): boolean {
    return this.auth.isAuthenticated();
  }

  protected async login(): Promise<void> {
    await this.auth.login();
  }

  protected async register(): Promise<void> {
    await this.auth.register();
  }

  protected async logout(): Promise<void> {
    await this.auth.logout();
  }
}
