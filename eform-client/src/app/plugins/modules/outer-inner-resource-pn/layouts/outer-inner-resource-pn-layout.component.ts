import { AfterContentInit, Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { LocaleService } from 'src/app/common/services/auth';
import { translates } from '../i18n/translates';

@Component({
  selector: 'app-machine-area-pn-layout',
  template: `<router-outlet></router-outlet>`,
})
export class OuterInnerResourcePnLayoutComponent
  implements AfterContentInit, OnInit {
  constructor(
    private localeService: LocaleService,
    private translateService: TranslateService
  ) {}

  ngOnInit() {}

  ngAfterContentInit() {
    const lang = this.localeService.getCurrentUserLocale();
    const i18n = translates[lang];
    this.translateService.setTranslation(lang, i18n, true);
  }
}
