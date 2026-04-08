import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Translation } from '../../shared/translation/translation';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home extends Translation {
  protected readonly auth = inject(AuthService);

  protected readonly features = [
    {
      icon: 'storefront',
      titleKey: 'home.featureCatalogTitle',
      descriptionKey: 'home.featureCatalogDescription'
    },
    {
      icon: 'verified_user',
      titleKey: 'home.featureSecurityTitle',
      descriptionKey: 'home.featureSecurityDescription'
    },
    {
      icon: 'devices',
      titleKey: 'home.featureResponsiveTitle',
      descriptionKey: 'home.featureResponsiveDescription'
    }
  ] as const;

  protected async login(): Promise<void> {
    await this.auth.login();
  }

  protected async register(): Promise<void> {
    await this.auth.register();
  }
}
